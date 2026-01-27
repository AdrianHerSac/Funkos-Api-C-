using FunkosApi.Models;
using HotChocolate.Types;

namespace FunkosAPI.GraphQL.Types;

public class FunkoType : ObjectType<Funko>
{
    protected override void Configure(IObjectTypeDescriptor<Funko> descriptor)
    {
        descriptor.Name("Funko");
        descriptor.Description("Entidad Funko");

        descriptor.Field(p => p.Id).Type<NonNullType<IdType>>().Description("El ID del funko");
        descriptor.Field(p => p.Nombre).Type<NonNullType<StringType>>().Description("El nombre del funko");
        descriptor.Field(p => p.Descripcion).Type<StringType>().Description("La descripción del funko");
        descriptor.Field(p => p.Precio).Type<NonNullType<DecimalType>>().Description("El precio del funko");
        descriptor.Field(p => p.Stock).Type<NonNullType<FloatType>>().Description("Cantidad en stock");
        descriptor.Field(p => p.Imagen).Type<StringType>().Description("URL de la imagen");
        descriptor.Field(p => p.Categoria).Type<NonNullType<StringType>>().Description("La categoría");
        descriptor.Field(p => p.FechaCreacion).Type<NonNullType<DateTimeType>>().Description("Fecha de creación");
        descriptor.Field(p => p.FechaModificacion).Type<NonNullType<DateTimeType>>().Description("Fecha de última actualización");
        descriptor.Field(p => p.IsDeleted).Type<NonNullType<BooleanType>>().Description("Si el producto está eliminado");
    }
}