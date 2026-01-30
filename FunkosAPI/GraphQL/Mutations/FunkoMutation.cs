using FunkosApi.dto;
using FunkosApi.Services;
using FunkosApi.Error;
using FunkosAPI.GraphQL.Inputs;
using CSharpFunctionalExtensions;
using HotChocolate;

namespace FunkosAPI.GraphQL.Mutations;

public class FunkoMutation
{
    public async Task<Result<FunkoResponseDto, FunkoError>> CreateFunko(
        CreateProductoInput input,
        [Service] IService service)
    {
        var dto = new FunkoRequestDto
        {
            Nombre = input.Nombre,
            Descripcion = input.Descripcion ?? string.Empty,
            Precio = (double)input.Precio,
            Stock = input.Stock,
            Imagen = input.Imagen ?? "https://via.placeholder.com/150",
            Categoria = input.CategoriaId.ToString()
        };

        return await service.SaveFunkoAsync(dto);
    }
    
    public async Task<Result<FunkoResponseDto, FunkoError>> UpdateFunko(
        string id,
        UpdateProductoInput input,
        [Service] IService service)
    {
        var dto = new FunkoRequestDto
        {
            Nombre = input.Nombre ?? string.Empty,
            Descripcion = input.Descripcion ?? string.Empty,
            Precio = input.Precio.HasValue ? (double)input.Precio.Value : 0,
            Stock = input.Stock ?? 0,
            Imagen = input.Imagen ?? string.Empty,
            Categoria = input.CategoriaId.HasValue ? input.CategoriaId.ToString() : string.Empty
        };

        return await service.UpdateFunkoAsync(id, dto);
    }

    public async Task<Result<FunkoResponseDto, FunkoError>> DeleteFunko(
        string id,
        [Service] IService service)
    {
        return await service.DeleteFunkoAsync(id);
    }
}