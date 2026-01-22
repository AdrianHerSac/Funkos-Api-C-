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
            funko.Imagen,
            funko.FechaCreacion,
            funko.FechaModificacion
        );
    }
}