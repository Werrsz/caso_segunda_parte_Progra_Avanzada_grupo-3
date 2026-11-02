using System.ComponentModel.DataAnnotations;
using Abstracciones.Comunes;

namespace Abstracciones.Asistentes;

public class AsistenteCrearDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    [StringLength(150)]
    public string Correo { get; set; } = string.Empty;
}

public class AsistenteActualizarDto : AsistenteCrearDto { }

public record AsistenteDto(
    int Id,
    string Nombre,
    string Correo,
    DateTime FechaRegistro);

public class AsistenteFiltro : ParametrosPaginacion
{
    [StringLength(100)]
    public string? Q { get; set; }
}
