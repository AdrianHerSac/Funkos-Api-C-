using FunkosApi.Models;

namespace FunkosApi.Repositories;

public interface IUserRepository
{
    Task<User?> FindByUsernameAsync(string username);
    Task<User> AddAsync(User user);
}
