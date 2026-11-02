using Abstracciones.Eventos;

namespace Flujo.Eventos;

public static class EventoMapeador
{
    public static EventoDto ADto(Evento e) =>
        new(e.Id, e.Titulo, e.Descripcion, e.Lugar, e.FechaInicio, e.FechaFin,
            e.CupoMaximo, e.Estado, e.ConferencistaId,
            e.Conferencista?.Nombre ?? string.Empty, e.FechaCreacion);
}
