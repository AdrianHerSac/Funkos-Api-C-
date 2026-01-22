namespace FunkosApi.dto;

public record FunkoResponseDto(
    string id,
    string nombre,
    double precio,
    string categoria,
    string? imagen,
    DateTime fechaCreacion,
    DateTime fechaModificacion)
{

    public string Id { get; set; } = id;
    public string Nombre { get; set; } = nombre;
    public double Precio { get; set; } = precio;
    public string Categoria { get; set; } = categoria;
    public string? Imagen { get; set; } = imagen;
    public DateTime FechaCreacion { get; set; } = fechaCreacion;
    public DateTime FechaModificacion { get; set; } = fechaModificacion;
};