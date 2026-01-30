using System.Net.Http.Json;
using ConsoleTables;

var baseUrl = "http://localhost:5084"; 
using var client = new HttpClient { BaseAddress = new Uri(baseUrl) };

Console.WriteLine("INICIANDO CLIENTE DE PRUEBAS FUNKOS");
Console.WriteLine($"Conectando a: {baseUrl}");
Console.WriteLine("------------------------------------------------");


try
{
    await ObtenerTodos(client, "Estado Inicial");

    var nuevoFunko = new FunkoRequest("Batman Cliente", 15.50, "DC", 10, "https://via.placeholder.com/150");
    Console.WriteLine($"\nCreando Funko: {nuevoFunko.Nombre}...");
    
    var responsePost = await client.PostAsJsonAsync("/api/funkos", nuevoFunko);
    
    if (responsePost.IsSuccessStatusCode)
    {
        var creado = await responsePost.Content.ReadFromJsonAsync<FunkoResponse>();
        Console.WriteLine($"Creado con éxito. ID: {creado?.Id}");

        if (creado != null)
        {
            Console.WriteLine($"\nActualizando precio de {creado.Nombre} a 99.99...");
            var updateFunko = new FunkoRequest(creado.Nombre, 99.99, creado.Categoria, 5, creado.Imagen);
            
            var responsePut = await client.PutAsJsonAsync($"/api/funkos/{creado.Id}", updateFunko);
            Console.WriteLine(responsePut.IsSuccessStatusCode ? "Actualizado correctamente." : "Error al actualizar.");

            var responseGet = await client.GetAsync($"/api/funkos/{creado.Id}");
            var funkoActualizado = await responseGet.Content.ReadFromJsonAsync<FunkoResponse>();
            Console.WriteLine($"Verificación: Precio actual es {funkoActualizado?.Precio}");

            Console.WriteLine($"\nBorrando el Funko...");
            var responseDelete = await client.DeleteAsync($"/api/funkos/{creado.Id}");
            Console.WriteLine(responseDelete.IsSuccessStatusCode ? "Borrado correctamente." : "Error al borrar.");
        }
    }
    else
    {
        Console.WriteLine($"Error al crear: {responsePost.StatusCode}");
        var error = await responsePost.Content.ReadAsStringAsync();
        Console.WriteLine(error);
    }

    await ObtenerTodos(client, "Estado Final");
}
catch (Exception ex)
{
    Console.WriteLine($"\nEXCEPCIÓN FATAL: Asegúrate de que la API esté corriendo.");
    Console.WriteLine($"Error: {ex.Message}");
}

Console.WriteLine("\nPresiona cualquier tecla para salir...");
Console.ReadKey();

static async Task ObtenerTodos(HttpClient client, string titulo)
{
    Console.WriteLine($"\nLISTADO DE FUNKOS ({titulo}):");
    try
    {
        var funkos = await client.GetFromJsonAsync<List<FunkoResponse>>("/api/funkos");
        
        var table = new ConsoleTable("ID", "NOMBRE", "PRECIO", "CATEGORIA");
        if (funkos != null)
        {
            foreach (var f in funkos)
            {
                table.AddRow(f.Id.Length > 4 ? f.Id.Substring(0, 4) + "..." : f.Id, f.Nombre, f.Precio, f.Categoria);
            }
        }
        table.Write();
        Console.WriteLine($"Total: {funkos?.Count ?? 0}");
    }
    catch (HttpRequestException)
    {
        Console.WriteLine("No se pudo conectar con la API para listar.");
    }
}

public record FunkoRequest(
    string Nombre, 
    double Precio, 
    string Categoria, 
    int Stock, 
    string Imagen, 
    string Descripcion = "Creado desde cliente C#"
);

public record FunkoResponse(
    string Id, 
    string Nombre, 
    double Precio, 
    string Categoria, 
    string Imagen
);