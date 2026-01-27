using FunkosApi.dto;
using FunkosApi.Models;

namespace FunkoApi.mapper;

public static class FunkosMapper
{
    public static FunkoResponseDto ToDto(this Funko funko)
    {
        return new FunkoResponseDto(
            funko.Id ?? string.Empty,
            funko.Nombre, 
            funko.Precio, 
            funko.Categoria,
            funko.Stock,
            funko.Descripcion,
            funko.Imagen,
            funko.IsDeleted,
            funko.FechaCreacion,
            funko.FechaModificacion
        );
    }
}