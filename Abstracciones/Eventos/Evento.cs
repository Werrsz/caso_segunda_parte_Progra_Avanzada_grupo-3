using Abstracciones.Conferencistas;
using Abstracciones.Inscripciones;

namespace Abstracciones.Eventos;

public enum EstadoEvento { Programado, EnCurso, Finalizado, Cancelado }

public class Evento
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string Lugar { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int CupoMaximo { get; set; }
    public EstadoEvento Estado { get; set; }
    public int ConferencistaId { get; set; }
    public DateTime FechaCreacion { get; set; }

    // Navegación N-1
    public Conferencista? Conferencista { get; set; }

    // Navegación 1-N con Inscripciones (Muchos a Muchos con Asistentes)
    public List<Inscripcion> Inscripciones { get; set; } = new();
}
