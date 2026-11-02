using Abstracciones.Comunes;

namespace Abstracciones.Eventos;

public interface IEventoFlujo
{
    Task<ResultadoPaginado<EventoDto>> ListarAsync(EventoFiltro filtro, CancellationToken ct);
    Task<EventoDto> ObtenerAsync(int id, CancellationToken ct);
    Task<EventoDto> CrearAsync(EventoCrearDto dto, CancellationToken ct);
    Task ActualizarAsync(int id, EventoActualizarDto dto, CancellationToken ct);
    Task CambiarEstadoAsync(int id, EstadoEvento nuevoEstado, CancellationToken ct);
    Task EliminarAsync(int id, CancellationToken ct);
}
