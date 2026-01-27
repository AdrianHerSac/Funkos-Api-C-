using FunkosApi.Models;

namespace FunkosApi.Repositories;

public interface IFunkoRepository
{
    Task<List<Funko>> GetAllAsync();
    Task<Funko?> GetByIdAsync(string id);
    Task<Funko?> UpdateAsync(string id, Funko newFunko);
    Task<Funko> AddAsync(Funko newFunko);
    Task<Funko?> DeleteAsync(string id);
    Task<Funko?> FindByNombreAsync(string nombre);
    IQueryable<Funko> FindAllAsNoTracking();
    Task<Funko?> FindByIdAsync(string id);
}