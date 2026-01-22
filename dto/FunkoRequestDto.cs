using System.ComponentModel.DataAnnotations;

namespace FunkosApi.dto;

public record FunkoRequestDto
{
    [Required(ErrorMessage = "Ingrese un nombre válido de funko")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Ingrese un nombre entre 2 y 100 caracteres")]
    public string Nombre { get; init; } = string.Empty;

    [Required(ErrorMessage = "Ingrese un precio válido de funko")]
    [Range(0.01, 9999.9, ErrorMessage = "Ingrese un precio válido de funko")]
    public double Precio { get; init; }

    [Required(ErrorMessage = "Ingrese categoría válida de funko")]
    public string Categoria { get; init; } = string.Empty;
    
    public string? Imagen { get; init; }
}