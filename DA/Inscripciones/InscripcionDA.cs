using Abstracciones.Comunes;
using Abstracciones.Excepciones;
using Abstracciones.Inscripciones;
using DA.Comunes;
using DA.Contexto;
using Microsoft.EntityFrameworkCore;

namespace DA.Inscripciones;

public class InscripcionDA : IInscripcionDA
{
    private readonly EventosContexto _ctx;

    public InscripcionDA(EventosContexto ctx) => _ctx = ctx;

    public async Task<ResultadoPaginado<Inscripcion>> BuscarAsync(InscripcionFiltro filtro, CancellationToken ct)
    {
        IQueryable<Inscripcion> consulta = _ctx.Inscripciones
            .AsNoTracking()
            .Include(i => i.Evento)
            .Include(i => i.Asistente);

        if (filtro.EventoId.HasValue)
            consulta = consulta.Where(i => i.EventoId == filtro.EventoId.Value);

        if (filtro.AsistenteId.HasValue)
            consulta = consulta.Where(i => i.AsistenteId == filtro.AsistenteId.Value);

        var total = await consulta.CountAsync(ct);

        var items = await consulta
            .OrderBy(i => i.FechaInscripcion).ThenBy(i => i.Id)
            .Skip((filtro.Pagina - 1) * filtro.Tamano)
            .Take(filtro.Tamano)
            .ToListAsync(ct);

        return new ResultadoPaginado<Inscripcion>
        {
            Items = items,
            Total = total,
            Pagina = filtro.Pagina,
            Tamano = filtro.Tamano
        };
    }

    public Task<Inscripcion?> ObtenerPorIdAsync(int id, CancellationToken ct) =>
        _ctx.Inscripciones
            .AsNoTracking()
            .Include(i => i.Evento)
            .Include(i => i.Asistente)
            .FirstOrDefaultAsync(i => i.Id == id, ct);

    public Task<bool> ExisteInscripcionAsync(int eventoId, int asistenteId, CancellationToken ct) =>
        _ctx.Inscripciones.AnyAsync(i => i.EventoId == eventoId && i.AsistenteId == asistenteId, ct);

    public Task<int> ContarPorEventoAsync(int eventoId, CancellationToken ct) =>
        _ctx.Inscripciones.CountAsync(i => i.EventoId == eventoId, ct);

    public async Task<Inscripcion> AgregarAsync(Inscripcion inscripcion, CancellationToken ct)
    {
        _ctx.Inscripciones.Add(inscripcion);
        try
        {
            await _ctx.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ErroresSql.EsDuplicado(ex))
        {
            throw new ConflictoException("El asistente ya se encuentra inscrito en este evento.");
        }
        return inscripcion;
    }

    public async Task<bool> EliminarAsync(int id, CancellationToken ct)
    {
        var existente = await _ctx.Inscripciones.FirstOrDefaultAsync(i => i.Id == id, ct);
        if (existente is null) return false;

        _ctx.Inscripciones.Remove(existente);
        await _ctx.SaveChangesAsync(ct);
        return true;
    }
}
