using Abstracciones.Asistentes;
using Abstracciones.Comunes;
using Abstracciones.Excepciones;

namespace Flujo.Asistentes;

public class AsistenteFlujo : IAsistenteFlujo
{
    private readonly IAsistenteDA _da;
    private readonly TimeProvider _reloj;

    public AsistenteFlujo(IAsistenteDA da, TimeProvider reloj)
    {
        _da = da;
        _reloj = reloj;
    }

    public async Task<ResultadoPaginado<AsistenteDto>> ListarAsync(AsistenteFiltro filtro, CancellationToken ct)
    {
        var pagina = await _da.BuscarAsync(filtro, ct);
        return new ResultadoPaginado<AsistenteDto>
        {
            Items = pagina.Items.Select(AsistenteMapeador.ADto).ToList(),
            Total = pagina.Total,
            Pagina = pagina.Pagina,
            Tamano = pagina.Tamano
        };
    }

    public async Task<AsistenteDto> ObtenerAsync(int id, CancellationToken ct)
    {
        var asistente = await _da.ObtenerPorIdAsync(id, ct)
            ?? throw new NoEncontradoException("Asistente", id);
        return AsistenteMapeador.ADto(asistente);
    }

    public async Task<AsistenteDto> CrearAsync(AsistenteCrearDto dto, CancellationToken ct)
    {
        var correo = NormalizarCorreo(dto.Correo);

        if (await _da.ObtenerPorCorreoAsync(correo, ct) is not null)
            throw new ConflictoException($"Ya existe un asistente con el correo '{correo}'.");

        var asistente = new Asistente
        {
            Nombre = dto.Nombre.Trim(),
            Correo = correo,
            FechaRegistro = _reloj.GetUtcNow().UtcDateTime
        };

        await _da.AgregarAsync(asistente, ct);
        return AsistenteMapeador.ADto(asistente);
    }

    public async Task ActualizarAsync(int id, AsistenteActualizarDto dto, CancellationToken ct)
    {
        var correo = NormalizarCorreo(dto.Correo);

        _ = await _da.ObtenerPorIdAsync(id, ct) ?? throw new NoEncontradoException("Asistente", id);

        var otro = await _da.ObtenerPorCorreoAsync(correo, ct);
        if (otro is not null && otro.Id != id)
            throw new ConflictoException($"Ya existe otro asistente con el correo '{correo}'.");

        var actualizado = await _da.ActualizarAsync(new Asistente
        {
            Id = id,
            Nombre = dto.Nombre.Trim(),
            Correo = correo
        }, ct);

        if (!actualizado) throw new NoEncontradoException("Asistente", id);
    }

    public async Task EliminarAsync(int id, CancellationToken ct)
    {
        _ = await _da.ObtenerPorIdAsync(id, ct) ?? throw new NoEncontradoException("Asistente", id);

        if (!await _da.EliminarAsync(id, ct))
            throw new NoEncontradoException("Asistente", id);
    }

    private static string NormalizarCorreo(string correo) => correo.Trim().ToLowerInvariant();
}
