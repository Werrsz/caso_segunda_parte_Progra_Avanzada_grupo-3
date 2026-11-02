using Abstracciones.Comunes;

namespace Abstracciones.Inscripciones;

public interface IInscripcionDA
{
    Task<ResultadoPaginado<Inscripcion>> BuscarAsync(InscripcionFiltro filtro, CancellationToken ct);
    Task<Inscripcion?> ObtenerPorIdAsync(int id, CancellationToken ct);
    Task<bool> ExisteInscripcionAsync(int eventoId, int asistenteId, CancellationToken ct);
    Task<int> ContarPorEventoAsync(int eventoId, CancellationToken ct);
    Task<Inscripcion> AgregarAsync(Inscripcion inscripcion, CancellationToken ct);
    Task<bool> EliminarAsync(int id, CancellationToken ct);
}
