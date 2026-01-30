using FunkosApi.config;
using FunkosApi.Models;
using MongoDB.Driver;

namespace FunkosApi.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoCollection<User> _users;

    public UserRepository(MongoDbContext context)
    {
        _users = context.Users;
    }

    public async Task<User?> FindByUsernameAsync(string username)
    {
        return await _users.Find(u => u.Username == username).FirstOrDefaultAsync();
    }

    public async Task<User> AddAsync(User user)
    {
        await _users.InsertOneAsync(user);
        return user;
    }
}
