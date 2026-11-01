using System.ComponentModel.DataAnnotations;
using Abstracciones.Comunes;

namespace Abstracciones.Conferencistas;

public class ConferencistaCrearDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    [StringLength(150)]
    public string Correo { get; set; } = string.Empty;

    [Required(ErrorMessage = "La especialidad es obligatoria.")]
    [StringLength(100)]
    public string Especialidad { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Biografia { get; set; }
}

public class ConferencistaActualizarDto : ConferencistaCrearDto { }

public record ConferencistaDto(
    int Id,
    string Nombre,
    string Correo,
    string Especialidad,
    string? Biografia,
    DateTime FechaRegistro);

public class ConferencistaFiltro : ParametrosPaginacion
{
    [StringLength(100)]
    public string? Q { get; set; }
}