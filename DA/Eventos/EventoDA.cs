using Abstracciones.Comunes;
using Abstracciones.Eventos;
using Abstracciones.Excepciones;
using DA.Comunes;
using DA.Contexto;
using Microsoft.EntityFrameworkCore;

namespace DA.Eventos;

public class EventoDA : IEventoDA
{
    private readonly EventosContexto _ctx;

    public EventoDA(EventosContexto ctx) => _ctx = ctx;

    public async Task<ResultadoPaginado<Evento>> BuscarAsync(EventoFiltro filtro, CancellationToken ct)
    {
        IQueryable<Evento> consulta = _ctx.Eventos.AsNoTracking().Include(e => e.Conferencista);

        if (!string.IsNullOrWhiteSpace(filtro.Q))
        {
            var texto = filtro.Q.Trim();
            consulta = consulta.Where(e => e.Titulo.Contains(texto) || e.Lugar.Contains(texto));
        }
        if (filtro.Estado is not null)
            consulta = consulta.Where(e => e.Estado == filtro.Estado);
        if (filtro.ConferencistaId is not null)
            consulta = consulta.Where(e => e.ConferencistaId == filtro.ConferencistaId);

        var total = await consulta.CountAsync(ct);

        var items = await consulta
            .OrderBy(e => e.FechaInicio).ThenBy(e => e.Id)
            .Skip((filtro.Pagina - 1) * filtro.Tamano)
            .Take(filtro.Tamano)
            .ToListAsync(ct);

        return new ResultadoPaginado<Evento>
        {
            Items = items,
            Total = total,
            Pagina = filtro.Pagina,
            Tamano = filtro.Tamano
        };
    }

    public Task<Evento?> ObtenerPorIdAsync(int id, CancellationToken ct) =>
        _ctx.Eventos.AsNoTracking().Include(e => e.Conferencista).FirstOrDefaultAsync(e => e.Id == id, ct);

    public async Task<Evento> AgregarAsync(Evento evento, CancellationToken ct)
    {
        _ctx.Eventos.Add(evento);
        await _ctx.SaveChangesAsync(ct);
        return evento;
    }

    public async Task<bool> ActualizarAsync(Evento datos, CancellationToken ct)
    {
        var existente = await _ctx.Eventos.FirstOrDefaultAsync(e => e.Id == datos.Id, ct);
        if (existente is null) return false;

        existente.Titulo = datos.Titulo;
        existente.Descripcion = datos.Descripcion;
        existente.Lugar = datos.Lugar;
        existente.FechaInicio = datos.FechaInicio;
        existente.FechaFin = datos.FechaFin;
        existente.CupoMaximo = datos.CupoMaximo;
        existente.ConferencistaId = datos.ConferencistaId;

        await _ctx.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> CambiarEstadoAsync(int id, EstadoEvento estado, CancellationToken ct)
    {
        var existente = await _ctx.Eventos.FirstOrDefaultAsync(e => e.Id == id, ct);
        if (existente is null) return false;

        existente.Estado = estado;
        await _ctx.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> EliminarAsync(int id, CancellationToken ct)
    {
        var existente = await _ctx.Eventos.FirstOrDefaultAsync(e => e.Id == id, ct);
        if (existente is null) return false;

        _ctx.Eventos.Remove(existente);
        try
        {
            await _ctx.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ErroresSql.EsLlaveForanea(ex))
        {
            throw new ConflictoException("El evento tiene inscripciones asociadas y no se puede eliminar.");
        }
        return true;
    }

    public Task<bool> ExisteTraslapeAsync(int conferencistaId, DateTime inicio, DateTime fin, int? excluirId, CancellationToken ct) =>
        _ctx.Eventos.AnyAsync(e =>
            e.ConferencistaId == conferencistaId &&
            e.Estado != EstadoEvento.Cancelado &&
            e.FechaInicio < fin &&
            e.FechaFin > inicio &&
            (excluirId == null || e.Id != excluirId), ct);
}
