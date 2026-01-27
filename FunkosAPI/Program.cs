using System.Text.Json.Serialization;
using FunkosApi.config;
using FunkosApi.Repositories;
using FunkosApi.Services;
using FunkosAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Configurar MongoDB
builder.Services.AddSingleton<MongoDbContext>();


builder.Services.AddControllers()
    .AddJsonOptions(options => 
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IFunkoRepository, FunkoRepository>();
builder.Services.AddScoped<IService, FunkoService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();
app.MapControllers(); 
app.MapGet("/", () => "¡Bienvenido a la API de Funkos!");
app.Run();

// Hacer la clase Program accesible para tests E2E
public partial class Program { }