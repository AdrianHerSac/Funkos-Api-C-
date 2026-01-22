using MongoDB.Driver;

namespace FunkosApi.config;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;
    
    public MongoDbContext(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MongoDb");
        var databaseName = configuration["ConnectionStrings:DatabaseName"];
        
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(databaseName);
    }
    
    public IMongoCollection<Models.Funko> Funkos => 
        _database.GetCollection<Models.Funko>("funkos");
}
