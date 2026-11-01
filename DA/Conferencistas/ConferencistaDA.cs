using Abstracciones.Comunes;
using Abstracciones.Conferencistas;
using Abstracciones.Excepciones;
using DA.Comunes;
using DA.Contexto;
using Microsoft.EntityFrameworkCore;

namespace DA.Conferencistas;

public class ConferencistaDA : IConferencistaDA
{
    private readonly EventosContexto _ctx;

    public ConferencistaDA(EventosContexto ctx) => _ctx = ctx;

    public async Task<ResultadoPaginado<Conferencista>> BuscarAsync(ConferencistaFiltro filtro, CancellationToken ct)
    {
        IQueryable<Conferencista> consulta = _ctx.Conferencistas.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(filtro.Q))
        {
            var texto = filtro.Q.Trim();
            consulta = consulta.Where(c =>
                c.Nombre.Contains(texto) ||
                c.Correo.Contains(texto) ||
                c.Especialidad.Contains(texto));
        }

        var total = await consulta.CountAsync(ct);

        // Orden estable (Nombre, luego Id) para que la paginación no repita ni se salte filas.
        var items = await consulta
            .OrderBy(c => c.Nombre).ThenBy(c => c.Id)
            .Skip((filtro.Pagina - 1) * filtro.Tamano)
            .Take(filtro.Tamano)
            .ToListAsync(ct);

        return new ResultadoPaginado<Conferencista>
        {
            Items = items,
            Total = total,
            Pagina = filtro.Pagina,
            Tamano = filtro.Tamano
        };
    }

    public Task<Conferencista?> ObtenerPorIdAsync(int id, CancellationToken ct) =>
        _ctx.Conferencistas.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, ct);

    public Task<Conferencista?> ObtenerPorCorreoAsync(string correo, CancellationToken ct) =>
        _ctx.Conferencistas.AsNoTracking().FirstOrDefaultAsync(c => c.Correo == correo, ct);

    public async Task<Conferencista> AgregarAsync(Conferencista conferencista, CancellationToken ct)
    {
        _ctx.Conferencistas.Add(conferencista);
        try
        {
            await _ctx.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ErroresSql.EsDuplicado(ex))
        {
            throw new ConflictoException($"Ya existe un conferencista con el correo '{conferencista.Correo}'.");
        }
        return conferencista;
    }

    public async Task<bool> ActualizarAsync(Conferencista datos, CancellationToken ct)
    {
        var existente = await _ctx.Conferencistas.FirstOrDefaultAsync(c => c.Id == datos.Id, ct);
        if (existente is null) return false;

        existente.Nombre = datos.Nombre;
        existente.Correo = datos.Correo;
        existente.Especialidad = datos.Especialidad;
        existente.Biografia = datos.Biografia;

        try
        {
            await _ctx.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ErroresSql.EsDuplicado(ex))
        {
            throw new ConflictoException($"Ya existe otro conferencista con el correo '{datos.Correo}'.");
        }
        return true;
    }

    public async Task<bool> EliminarAsync(int id, CancellationToken ct)
    {
        var existente = await _ctx.Conferencistas.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (existente is null) return false;

        _ctx.Conferencistas.Remove(existente);
        try
        {
            await _ctx.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ErroresSql.EsLlaveForanea(ex))
        {
            throw new ConflictoException("El conferencista tiene eventos asociados y no se puede eliminar.");
        }
        return true;
    }
}
