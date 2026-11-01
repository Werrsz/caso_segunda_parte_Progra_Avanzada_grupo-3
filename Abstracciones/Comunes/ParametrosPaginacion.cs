using System.ComponentModel.DataAnnotations;

namespace Abstracciones.Comunes;

/// <summary>Parámetros de paginación comunes para todos los filtros de listado.</summary>
public class ParametrosPaginacion
{
    [Range(1, int.MaxValue, ErrorMessage = "La página debe ser 1 o mayor.")]
    public int Pagina { get; set; } = 1;

    [Range(1, 100, ErrorMessage = "El tamaño de página debe estar entre 1 y 100.")]
    public int Tamano { get; set; } = 20;
}
