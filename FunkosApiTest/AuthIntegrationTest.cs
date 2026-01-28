using System.Net;
using System.Net.Http.Json;
using FunkosApi.dto;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.MongoDb;
using Microsoft.Extensions.DependencyInjection;
using FunkosApi.config;
using MongoDB.Driver;

namespace FunkosApiTest;

[TestFixture]
public class AuthIntegrationTest
{
    private MongoDbContainer _mongoDbContainer;
    private WebApplicationFactory<Program> _factory;
    
#pragma warning disable NUnit1032
    
    private HttpClient _client;
    
#pragma warning restore NUnit1032

    [OneTimeSetUp]
    public async Task OneTimeSetUp()
    {
        try
        {
            _mongoDbContainer = new MongoDbBuilder()
                .WithImage("mongo:latest")
                .Build();
            await _mongoDbContainer.StartAsync();

            _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var mongoClient = new MongoClient(_mongoDbContainer.GetConnectionString());
                    services.AddSingleton<IMongoClient>(mongoClient);
                    
                    // Replace MongoDbContext if necessary or ensure it uses the connection string
                    services.AddSingleton(sp => new MongoDbContext(_mongoDbContainer.GetConnectionString()));
                });
            });

            _client = _factory.CreateClient();
        }
        catch (Exception ex) when (ex.GetType().Name.Contains("DockerUnavailableException") || ex.Message.Contains("Docker"))
        {
            Assert.Ignore("Docker is not available. Skipping integration tests.");
        }
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown()
    {
        _client?.Dispose();
        if (_mongoDbContainer != null)
        {
            await _mongoDbContainer.DisposeAsync();
        }
        _factory?.Dispose();
    }

    [Test]
    public async Task Auth_Flow_Verification()
    {
        var adminRegister = new RegisterRequestDto { Username = "admin", Password = "password", Role = "Admin" };
        var registerAdminRes = await _client.PostAsJsonAsync("/api/auth/register", adminRegister);
        Assert.That(registerAdminRes.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var userRegister = new RegisterRequestDto { Username = "user", Password = "password", Role = "User" };
        var registerUserRes = await _client.PostAsJsonAsync("/api/auth/register", userRegister);
        Assert.That(registerUserRes.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var adminLogin = new LoginRequestDto { Username = "admin", Password = "password" };
        var loginAdminRes = await _client.PostAsJsonAsync("/api/auth/login", adminLogin);
        Assert.That(loginAdminRes.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        var adminToken = (await loginAdminRes.Content.ReadFromJsonAsync<LoginResponseDto>())!.Token;

        var userLogin = new LoginRequestDto { Username = "user", Password = "password" };
        var loginUserRes = await _client.PostAsJsonAsync("/api/auth/login", userLogin);
        Assert.That(loginUserRes.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        
        var userToken = (await loginUserRes.Content.ReadFromJsonAsync<LoginResponseDto>())!.Token;
        var getRes = await _client.GetAsync("/api/funkos");
        Assert.That(getRes.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        var deleteNoAuth = await _client.DeleteAsync("/api/funkos/someid");
        Assert.That(deleteNoAuth.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", userToken);
        var deleteUserAuth = await _client.DeleteAsync("/api/funkos/someid");
        Assert.That(deleteUserAuth.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));

        _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", adminToken);
        var deleteAdminAuth = await _client.DeleteAsync("/api/funkos/someid");
        Assert.That(deleteAdminAuth.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
}