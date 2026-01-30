using FunkosApi.config;
using FunkosApi.Models;
using MongoDB.Driver;
using NLog;

namespace FunkosApi.Repositories;

public class FunkoRepository : IFunkoRepository
{
    private static Logger _log = LogManager.GetCurrentClassLogger();
    private readonly IMongoCollection<Funko> _funkos;
    
    public FunkoRepository(MongoDbContext context)
    {
        _funkos = context.Funkos;
    }
    
    public async Task<List<Funko>> GetAllAsync()
    {
        _log.Info("Getting all Funkos");
        return await _funkos.Find(_ => true).ToListAsync();
    }

    public async Task<Funko?> GetByIdAsync(string id)
    {
        _log.Info("Getting Funko with id: " + id);
        try
        {
            return await _funkos.Find(f => f.Id == id).FirstOrDefaultAsync();
        }
        catch (FormatException)
        {
            return null;
        }
    }

    public async Task<Funko?> UpdateAsync(string id, Funko newFunko)
    {
        _log.Info("Updating Funko with id: " + id);
        
        try
        {
            var filter = Builders<Funko>.Filter.Eq(f => f.Id, id);
            var update = Builders<Funko>.Update
                .Set(f => f.Nombre, newFunko.Nombre)
                .Set(f => f.Categoria, newFunko.Categoria)
                .Set(f => f.Precio, newFunko.Precio)
                .Set(f => f.Stock, newFunko.Stock)
                .Set(f => f.Descripcion, newFunko.Descripcion)
                .Set(f => f.Imagen, newFunko.Imagen)
                .Set(f => f.FechaModificacion, DateTime.Now);
            
            var options = new FindOneAndUpdateOptions<Funko>
            {
                ReturnDocument = ReturnDocument.After
            };
            
            return await _funkos.FindOneAndUpdateAsync(filter, update, options);
        }
        catch (FormatException)
        {
            return null;
        }
    }

    public async Task<Funko> AddAsync(Funko newFunko)
    { 
        _log.Info("Adding Funko");
        newFunko.FechaCreacion = DateTime.Now;
        newFunko.FechaModificacion = DateTime.Now;
        await _funkos.InsertOneAsync(newFunko);
        return newFunko;
    }

    public async Task<Funko?> DeleteAsync(string id)
    {
        _log.Info("Deleting Funko with id: " + id);
        try
        {
            var filter = Builders<Funko>.Filter.Eq(f => f.Id, id);
            return await _funkos.FindOneAndDeleteAsync(filter);
        }
        catch (FormatException)
        {
            return null;
        }
    }

    public async Task<Funko?> FindByNombreAsync(string nombre)
    {
        return await _funkos.Find(f => f.Nombre == nombre).FirstOrDefaultAsync();
    }

    public IQueryable<Funko> FindAllAsNoTracking()
    {
        return _funkos.AsQueryable();
    }

    public Task<Funko?> FindByIdAsync(string id)
    {
        try
        {
            return _funkos.Find(f => f.Id == id).FirstOrDefaultAsync();
        }
        catch (FormatException)
        {
            return Task.FromResult<Funko?>(null);
        }
    }
}