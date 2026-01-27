namespace FunkosApi.dto;

public record FunkoResponseDto(
    string Id,
    string Nombre,
    double Precio,
    string Categoria,
    double Stock,
    string Descripcion,
    string? Imagen,
    bool IsDeleted,
    DateTime FechaCreacion,
    DateTime FechaModificacion
);