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
    
    [Required(ErrorMessage = "Ingrese un stock válido")]
    [Range(0, 99999, ErrorMessage = "Ingrese un stock entre 0 y 99999")]
    public double Stock { get; init; }
    
    public string? Descripcion { get; init; }
    
    public string? Imagen { get; init; }
}