using MongoDB.Driver;

namespace FunkosApi.config;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;
    
    // Constructor para producción
    public MongoDbContext(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MongoDb");
        var databaseName = configuration["ConnectionStrings:DatabaseName"];
        
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }
    
    // Constructor para tests E2E (sobrecarga)
    public MongoDbContext(string connectionString, string databaseName = "FunkosTestDB")
    {
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }
    
    public IMongoCollection<Models.Funko> Funkos => 
        _database.GetCollection<Models.Funko>("funkos");
        
    public IMongoCollection<Models.User> Users =>
        _database.GetCollection<Models.User>("users");
}
