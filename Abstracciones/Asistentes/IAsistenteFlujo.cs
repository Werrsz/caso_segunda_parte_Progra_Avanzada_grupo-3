using Abstracciones.Comunes;

namespace Abstracciones.Asistentes;

public interface IAsistenteFlujo
{
    Task<ResultadoPaginado<AsistenteDto>> ListarAsync(AsistenteFiltro filtro, CancellationToken ct);
    Task<AsistenteDto> ObtenerAsync(int id, CancellationToken ct);
    Task<AsistenteDto> CrearAsync(AsistenteCrearDto dto, CancellationToken ct);
    Task ActualizarAsync(int id, AsistenteActualizarDto dto, CancellationToken ct);
    Task EliminarAsync(int id, CancellationToken ct);
}
