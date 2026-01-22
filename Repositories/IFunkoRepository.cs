using FunkosApi.Models;

namespace FunkosApi.Repository;

public interface IFunkoRepository
{
    Task<List<Funko>> GetAllAsync();
    Task<Funko?> GetByIdAsync(string id);
    Task<Funko?> UpdateAsync(string id, Funko newFunko);
    Task<Funko> AddAsync(Funko newFunko);
    Task<Funko?> DeleteAsync(string id);
    Task<Funko?> FindByNombreAsync(string nombre);
}