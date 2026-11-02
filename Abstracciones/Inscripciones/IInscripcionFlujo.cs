using Abstracciones.Comunes;

namespace Abstracciones.Inscripciones;

public interface IInscripcionFlujo
{
    Task<ResultadoPaginado<InscripcionDto>> ListarAsync(InscripcionFiltro filtro, CancellationToken ct);
    Task<InscripcionDto> ObtenerAsync(int id, CancellationToken ct);
    Task<InscripcionDto> CrearAsync(InscripcionCrearDto dto, CancellationToken ct);
    Task EliminarAsync(int id, CancellationToken ct);
}
