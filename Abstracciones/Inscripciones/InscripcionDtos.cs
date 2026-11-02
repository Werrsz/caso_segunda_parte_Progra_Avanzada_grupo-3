using System.ComponentModel.DataAnnotations;
using Abstracciones.Comunes;

namespace Abstracciones.Inscripciones;

public class InscripcionCrearDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Debe indicar un evento válido.")]
    public int EventoId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe indicar un asistente válido.")]
    public int AsistenteId { get; set; }
}

public record InscripcionDto(
    int Id,
    int EventoId,
    string EventoTitulo,
    int AsistenteId,
    string AsistenteNombre,
    string AsistenteCorreo,
    DateTime FechaInscripcion);

public class InscripcionFiltro : ParametrosPaginacion
{
    public int? EventoId { get; set; }
    public int? AsistenteId { get; set; }
}
