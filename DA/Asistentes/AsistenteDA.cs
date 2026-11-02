using Abstracciones.Asistentes;
using Abstracciones.Comunes;
using Abstracciones.Excepciones;
using DA.Comunes;
using DA.Contexto;
using Microsoft.EntityFrameworkCore;

namespace DA.Asistentes;

public class AsistenteDA : IAsistenteDA
{
    private readonly EventosContexto _ctx;

    public AsistenteDA(EventosContexto ctx) => _ctx = ctx;

    public async Task<ResultadoPaginado<Asistente>> BuscarAsync(AsistenteFiltro filtro, CancellationToken ct)
    {
        IQueryable<Asistente> consulta = _ctx.Asistentes.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filtro.Q))
        {
            var texto = filtro.Q.Trim();
            consulta = consulta.Where(a => a.Nombre.Contains(texto) || a.Correo.Contains(texto));
        }

        var total = await consulta.CountAsync(ct);

        var items = await consulta
            .OrderBy(a => a.Nombre).ThenBy(a => a.Id)
            .Skip((filtro.Pagina - 1) * filtro.Tamano)
            .Take(filtro.Tamano)
            .ToListAsync(ct);

        return new ResultadoPaginado<Asistente>
        {
            Items = items,
            Total = total,
            Pagina = filtro.Pagina,
            Tamano = filtro.Tamano
        };
    }

    public Task<Asistente?> ObtenerPorIdAsync(int id, CancellationToken ct) =>
        _ctx.Asistentes.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id, ct);

    public Task<Asistente?> ObtenerPorCorreoAsync(string correo, CancellationToken ct) =>
        _ctx.Asistentes.AsNoTracking().FirstOrDefaultAsync(a => a.Correo == correo, ct);

    public async Task<Asistente> AgregarAsync(Asistente asistente, CancellationToken ct)
    {
        _ctx.Asistentes.Add(asistente);
        try
        {
            await _ctx.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ErroresSql.EsDuplicado(ex))
        {
            throw new ConflictoException($"Ya existe un asistente con el correo '{asistente.Correo}'.");
        }
        return asistente;
    }

    public async Task<bool> ActualizarAsync(Asistente datos, CancellationToken ct)
    {
        var existente = await _ctx.Asistentes.FirstOrDefaultAsync(a => a.Id == datos.Id, ct);
        if (existente is null) return false;

        existente.Nombre = datos.Nombre;
        existente.Correo = datos.Correo;

        try
        {
            await _ctx.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ErroresSql.EsDuplicado(ex))
        {
            throw new ConflictoException($"Ya existe otro asistente con el correo '{datos.Correo}'.");
        }
        return true;
    }

    public async Task<bool> EliminarAsync(int id, CancellationToken ct)
    {
        var existente = await _ctx.Asistentes.FirstOrDefaultAsync(a => a.Id == id, ct);
        if (existente is null) return false;

        _ctx.Asistentes.Remove(existente);
        try
        {
            await _ctx.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ErroresSql.EsLlaveForanea(ex))
        {
            throw new ConflictoException("El asistente tiene inscripciones asociadas y no se puede eliminar.");
        }
        return true;
    }
}
