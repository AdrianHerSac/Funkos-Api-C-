using System.Net.Http.Json;
using FluentAssertions;
using FunkosApi.dto;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.MongoDb;

namespace FunkosApiTestE2E;

[TestFixture]
public class FunkosE2ETest
{
    private MongoDbContainer _mongoDbContainer;
    
    private WebApplicationFactory<Program> _factory;
    
    private HttpClient _client;

    [OneTimeSetUp]
    public async Task GlobalSetup()
    {
        _mongoDbContainer = new MongoDbBuilder()
            .WithImage("mongo:7.0") 
            .WithUsername("root")  
            .WithPassword("password123") 
            .Build();

        await _mongoDbContainer.StartAsync();

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                var connectionString = _mongoDbContainer.GetConnectionString();
                
                builder.UseSetting("ConnectionStrings:MongoDb", connectionString);
                builder.UseSetting("ConnectionStrings:DatabaseName", "FunkosDb_E2E");
            });

        _client = _factory.CreateClient();
    }

    [OneTimeTearDown] 
    public async Task GlobalTeardown()
    {
        _client.Dispose();
        await _factory.DisposeAsync();
        
        await _mongoDbContainer.StopAsync();
        
        await _mongoDbContainer.DisposeAsync();
    }

    [Test]
    public async Task GetFunkos_DeberiaDevolverListaVacia_AlInicio()
    {
        var response = await _client.GetAsync("/api/funkos");

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var funkos = await response.Content.ReadFromJsonAsync<List<FunkoResponseDto>>();
        funkos.Should().NotBeNull();
        funkos.Should().BeEmpty("la base de datos acaba de nacer y debería estar vacía");
    }

    [Test]
    public async Task PostFunko_DeberiaCrearElemento_Y_Devolverlo()
    {
        var nuevoFunko = new FunkoRequestDto 
        { 
            Nombre = "Funko E2E", 
            Precio = 19.99, 
            Stock = 10, 
            Categoria = "Test",
            Imagen = "http://img.com",
            Descripcion = "Creado en test E2E"
        };

        var response = await _client.PostAsJsonAsync("/api/funkos", nuevoFunko);

        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created); 

        var creado = await response.Content.ReadFromJsonAsync<FunkoResponseDto>();
        creado.Should().NotBeNull();
        creado!.Nombre.Should().Be("Funko E2E");
        creado.Id.Should().NotBeNullOrEmpty();
    }
}