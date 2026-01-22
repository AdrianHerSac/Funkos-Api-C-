using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FunkosApi.Models;

public record Funko
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [BsonElement("categoria")]
    public string Categoria { get; set; } = string.Empty;

    [BsonElement("precio")]
    public double Precio { get; set; }

    [BsonElement("fechaCreacion")]
    public DateTime FechaCreacion { get; set; } = DateTime.Now;
    
    [BsonElement("fechaModificacion")]
    public DateTime FechaModificacion { get; set; } = DateTime.Now;

    [BsonElement("imagen")]
    public string? Imagen { get; set; }
}