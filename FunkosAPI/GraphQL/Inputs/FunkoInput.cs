namespace FunkosAPI.GraphQL.Inputs;

public record CreateProductoInput
{
    public string Nombre { get; init; } = string.Empty;

    public string? Descripcion { get; init; }

    public decimal Precio { get; init; }

    public int Stock { get; init; }

    public string? Imagen { get; init; }

    public long CategoriaId { get; init; }
}

public record UpdateProductoInput
{
    public string? Nombre { get; init; }

    public string? Descripcion { get; init; }

    public decimal? Precio { get; init; }

    public int? Stock { get; init; }

    public string? Imagen { get; init; }

    public long? CategoriaId { get; init; }
}