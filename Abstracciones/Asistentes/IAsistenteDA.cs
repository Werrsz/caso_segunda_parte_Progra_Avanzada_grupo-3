using Abstracciones.Comunes;

namespace Abstracciones.Asistentes;

public interface IAsistenteDA
{
    Task<ResultadoPaginado<Asistente>> BuscarAsync(AsistenteFiltro filtro, CancellationToken ct);
    Task<Asistente?> ObtenerPorIdAsync(int id, CancellationToken ct);
    Task<Asistente?> ObtenerPorCorreoAsync(string correo, CancellationToken ct);
    Task<Asistente> AgregarAsync(Asistente asistente, CancellationToken ct);
    Task<bool> ActualizarAsync(Asistente datos, CancellationToken ct);
    Task<bool> EliminarAsync(int id, CancellationToken ct);
}
