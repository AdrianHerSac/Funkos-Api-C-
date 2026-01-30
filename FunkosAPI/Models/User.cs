using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FunkosApi.Models;

public enum Role
{
    User,
    Admin
}

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty; 
    
    [BsonRepresentation(BsonType.String)]
    public Role Role { get; set; } = Role.User;
}
