using Abstracciones.Asistentes;
using Abstracciones.Eventos;

namespace Abstracciones.Inscripciones;

public class Inscripcion
{
    public int Id { get; set; }
    public int EventoId { get; set; }
    public int AsistenteId { get; set; }
    public DateTime FechaInscripcion { get; set; }

    // Navegaciones N-1
    public Evento? Evento { get; set; }
    public Asistente? Asistente { get; set; }
}
