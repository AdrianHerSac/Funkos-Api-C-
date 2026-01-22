namespace FunkosApi.dto;

public record FunkoResponseDto(
    string Id,
    string Nombre,
    double Precio,
    string Categoria,
    string? Imagen,
    DateTime FechaCreacion,
    DateTime FechaModificacion
);