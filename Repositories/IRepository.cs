using FunkosApi.Models;

namespace FunkosApi.Repository;

public interface IRepository<T,ID>
{
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdAsync(ID id);
    
}