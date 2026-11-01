using Abstracciones.Comunes;

namespace Abstracciones.Conferencistas;

public interface IConferencistaDA
{
    Task<ResultadoPaginado<Conferencista>> BuscarAsync(ConferencistaFiltro filtro, CancellationToken ct);
    Task<Conferencista?> ObtenerPorIdAsync(int id, CancellationToken ct);
    Task<Conferencista?> ObtenerPorCorreoAsync(string correo, CancellationToken ct);
    Task<Conferencista> AgregarAsync(Conferencista conferencista, CancellationToken ct);
    Task<bool> ActualizarAsync(Conferencista datos, CancellationToken ct);
    Task<bool> EliminarAsync(int id, CancellationToken ct);
}