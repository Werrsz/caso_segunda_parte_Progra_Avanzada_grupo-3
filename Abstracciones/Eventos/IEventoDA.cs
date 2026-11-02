using Abstracciones.Comunes;

namespace Abstracciones.Eventos;

public interface IEventoDA
{
    Task<ResultadoPaginado<Evento>> BuscarAsync(EventoFiltro filtro, CancellationToken ct);
    Task<Evento?> ObtenerPorIdAsync(int id, CancellationToken ct);
    Task<Evento> AgregarAsync(Evento evento, CancellationToken ct);
    Task<bool> ActualizarAsync(Evento datos, CancellationToken ct);
    Task<bool> CambiarEstadoAsync(int id, EstadoEvento estado, CancellationToken ct);
    Task<bool> EliminarAsync(int id, CancellationToken ct);
    Task<bool> ExisteTraslapeAsync(int conferencistaId, DateTime inicio, DateTime fin, int? excluirId, CancellationToken ct);
}
