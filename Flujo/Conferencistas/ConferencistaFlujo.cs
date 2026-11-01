using Abstracciones.Comunes;
using Abstracciones.Conferencistas;
using Abstracciones.Excepciones;

namespace Flujo.Conferencistas;

public class ConferencistaFlujo : IConferencistaFlujo
{
    private readonly IConferencistaDA _da;
    private readonly TimeProvider _reloj;

    // TimeProvider en vez de DateTime.UtcNow directo: en las pruebas se puede controlar el reloj.
    public ConferencistaFlujo(IConferencistaDA da, TimeProvider reloj)
    {
        _da = da;
        _reloj = reloj;
    }

    public async Task<ResultadoPaginado<ConferencistaDto>> ListarAsync(ConferencistaFiltro filtro, CancellationToken ct)
    {
        var pagina = await _da.BuscarAsync(filtro, ct);
        return new ResultadoPaginado<ConferencistaDto>
        {
            Items = pagina.Items.Select(ConferencistaMapeador.ADto).ToList(),
            Total = pagina.Total,
            Pagina = pagina.Pagina,
            Tamano = pagina.Tamano
        };
    }

    public async Task<ConferencistaDto> ObtenerAsync(int id, CancellationToken ct)
    {
        var c = await _da.ObtenerPorIdAsync(id, ct)
            ?? throw new NoEncontradoException("Conferencista", id);
        return ConferencistaMapeador.ADto(c);
    }

    public async Task<ConferencistaDto> CrearAsync(ConferencistaCrearDto dto, CancellationToken ct)
    {
        var correo = NormalizarCorreo(dto.Correo);

        if (await _da.ObtenerPorCorreoAsync(correo, ct) is not null)
            throw new ConflictoException($"Ya existe un conferencista con el correo '{correo}'.");

        var conferencista = new Conferencista
        {
            Nombre = dto.Nombre.Trim(),
            Correo = correo,
            Especialidad = dto.Especialidad.Trim(),
            Biografia = LimpiarOpcional(dto.Biografia),
            FechaRegistro = _reloj.GetUtcNow().UtcDateTime
        };

        await _da.AgregarAsync(conferencista, ct);
        return ConferencistaMapeador.ADto(conferencista);
    }

    public async Task ActualizarAsync(int id, ConferencistaActualizarDto dto, CancellationToken ct)
    {
        var correo = NormalizarCorreo(dto.Correo);

        _ = await _da.ObtenerPorIdAsync(id, ct) ?? throw new NoEncontradoException("Conferencista", id);

        // El correo puede repetirse solo si pertenece al MISMO conferencista que se está editando.
        var otro = await _da.ObtenerPorCorreoAsync(correo, ct);
        if (otro is not null && otro.Id != id)
            throw new ConflictoException($"Ya existe otro conferencista con el correo '{correo}'.");

        var actualizado = await _da.ActualizarAsync(new Conferencista
        {
            Id = id,
            Nombre = dto.Nombre.Trim(),
            Correo = correo,
            Especialidad = dto.Especialidad.Trim(),
            Biografia = LimpiarOpcional(dto.Biografia)
        }, ct);

        if (!actualizado) throw new NoEncontradoException("Conferencista", id);
    }

    public async Task EliminarAsync(int id, CancellationToken ct)
    {
        _ = await _da.ObtenerPorIdAsync(id, ct) ?? throw new NoEncontradoException("Conferencista", id);

        if (!await _da.EliminarAsync(id, ct))
            throw new NoEncontradoException("Conferencista", id);
    }

    private static string NormalizarCorreo(string correo) => correo.Trim().ToLowerInvariant();

    private static string? LimpiarOpcional(string? texto) =>
        string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();
}
