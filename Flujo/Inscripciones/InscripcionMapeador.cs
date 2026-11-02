using Abstracciones.Inscripciones;

namespace Flujo.Inscripciones;

public static class InscripcionMapeador
{
    public static InscripcionDto ADto(Inscripcion i) =>
        new(
            i.Id,
            i.EventoId,
            i.Evento?.Titulo ?? string.Empty,
            i.AsistenteId,
            i.Asistente?.Nombre ?? string.Empty,
            i.Asistente?.Correo ?? string.Empty,
            i.FechaInscripcion
        );
}
