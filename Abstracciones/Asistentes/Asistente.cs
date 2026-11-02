using Abstracciones.Inscripciones;

namespace Abstracciones.Asistentes;

public class Asistente
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; }

    // Relación 1-N con Inscripciones (Muchos a Muchos con Eventos)
    public List<Inscripcion> Inscripciones { get; set; } = new();
}
