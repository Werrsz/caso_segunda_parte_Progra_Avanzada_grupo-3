using Abstracciones.Comunes;

namespace Abstracciones.Conferencistas;

public interface IConferencistaFlujo
{
    Task<ResultadoPaginado<ConferencistaDto>> ListarAsync(ConferencistaFiltro filtro, CancellationToken ct);
    Task<ConferencistaDto> ObtenerAsync(int id, CancellationToken ct);
    Task<ConferencistaDto> CrearAsync(ConferencistaCrearDto dto, CancellationToken ct);
    Task ActualizarAsync(int id, ConferencistaActualizarDto dto, CancellationToken ct);
    Task EliminarAsync(int id, CancellationToken ct);
}