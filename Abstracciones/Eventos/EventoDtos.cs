using System.ComponentModel.DataAnnotations;
using Abstracciones.Comunes;

namespace Abstracciones.Eventos;

public class EventoCrearDto
{
    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "El título debe tener entre 3 y 150 caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Descripcion { get; set; }

    [Required(ErrorMessage = "El lugar es obligatorio.")]
    [StringLength(150)]
    public string Lugar { get; set; } = string.Empty;

    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }

    [Range(1, 1000, ErrorMessage = "El cupo debe estar entre 1 y 1000.")]
    public int CupoMaximo { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe indicar un conferencista.")]
    public int ConferencistaId { get; set; }
}

public class EventoActualizarDto : EventoCrearDto { }

public class EventoCambiarEstadoDto
{
    public EstadoEvento Estado { get; set; }
}

public record EventoDto(
    int Id,
    string Titulo,
    string? Descripcion,
    string Lugar,
    DateTime FechaInicio,
    DateTime FechaFin,
    int CupoMaximo,
    EstadoEvento Estado,
    int ConferencistaId,
    string ConferencistaNombre,
    DateTime FechaCreacion);

public class EventoFiltro : ParametrosPaginacion
{
    [StringLength(100)]
    public string? Q { get; set; }
    public EstadoEvento? Estado { get; set; }
    public int? ConferencistaId { get; set; }
}
