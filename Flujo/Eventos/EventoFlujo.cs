using Abstracciones.Comunes;
using Abstracciones.Conferencistas;
using Abstracciones.Eventos;
using Abstracciones.Excepciones;

namespace Flujo.Eventos;

public class EventoFlujo : IEventoFlujo
{
    private readonly IEventoDA _da;
    private readonly IConferencistaDA _conferencistaDA;
    private readonly TimeProvider _reloj;

    public EventoFlujo(IEventoDA da, IConferencistaDA conferencistaDA, TimeProvider reloj)
    {
        _da = da;
        _conferencistaDA = conferencistaDA;
        _reloj = reloj;
    }

    public async Task<ResultadoPaginado<EventoDto>> ListarAsync(EventoFiltro filtro, CancellationToken ct)
    {
        var pagina = await _da.BuscarAsync(filtro, ct);
        return new ResultadoPaginado<EventoDto>
        {
            Items = pagina.Items.Select(EventoMapeador.ADto).ToList(),
            Total = pagina.Total,
            Pagina = pagina.Pagina,
            Tamano = pagina.Tamano
        };
    }

    public async Task<EventoDto> ObtenerAsync(int id, CancellationToken ct)
    {
        var evento = await _da.ObtenerPorIdAsync(id, ct)
            ?? throw new NoEncontradoException("Evento", id);
        return EventoMapeador.ADto(evento);
    }

    public async Task<EventoDto> CrearAsync(EventoCrearDto dto, CancellationToken ct)
    {
        await ValidarReglasAsync(dto, null, ct);

        var evento = new Evento
        {
            Titulo = dto.Titulo.Trim(),
            Descripcion = LimpiarOpcional(dto.Descripcion),
            Lugar = dto.Lugar.Trim(),
            FechaInicio = dto.FechaInicio,
            FechaFin = dto.FechaFin,
            CupoMaximo = dto.CupoMaximo,
            Estado = EstadoEvento.Programado,
            ConferencistaId = dto.ConferencistaId,
            FechaCreacion = _reloj.GetUtcNow().UtcDateTime
        };

        await _da.AgregarAsync(evento, ct);

        // Se vuelve a leer para traer el nombre del conferencista (Include).
        var creado = await _da.ObtenerPorIdAsync(evento.Id, ct)
            ?? throw new NoEncontradoException("Evento", evento.Id);
        return EventoMapeador.ADto(creado);
    }

    public async Task ActualizarAsync(int id, EventoActualizarDto dto, CancellationToken ct)
    {
        var existente = await _da.ObtenerPorIdAsync(id, ct)
            ?? throw new NoEncontradoException("Evento", id);

        if (existente.Estado != EstadoEvento.Programado)
            throw new ConflictoException("Solo se pueden editar eventos en estado Programado.");

        await ValidarReglasAsync(dto, id, ct);

        var actualizado = await _da.ActualizarAsync(new Evento
        {
            Id = id,
            Titulo = dto.Titulo.Trim(),
            Descripcion = LimpiarOpcional(dto.Descripcion),
            Lugar = dto.Lugar.Trim(),
            FechaInicio = dto.FechaInicio,
            FechaFin = dto.FechaFin,
            CupoMaximo = dto.CupoMaximo,
            ConferencistaId = dto.ConferencistaId
        }, ct);

        if (!actualizado) throw new NoEncontradoException("Evento", id);
    }

    public async Task CambiarEstadoAsync(int id, EstadoEvento nuevoEstado, CancellationToken ct)
    {
        var existente = await _da.ObtenerPorIdAsync(id, ct)
            ?? throw new NoEncontradoException("Evento", id);

        if (!TransicionValida(existente.Estado, nuevoEstado))
            throw new ConflictoException($"No se puede pasar un evento de {existente.Estado} a {nuevoEstado}.");

        if (!await _da.CambiarEstadoAsync(id, nuevoEstado, ct))
            throw new NoEncontradoException("Evento", id);
    }

    public async Task EliminarAsync(int id, CancellationToken ct)
    {
        _ = await _da.ObtenerPorIdAsync(id, ct) ?? throw new NoEncontradoException("Evento", id);

        if (!await _da.EliminarAsync(id, ct))
            throw new NoEncontradoException("Evento", id);
    }

    // ---------- Reglas ----------

    private async Task ValidarReglasAsync(EventoCrearDto dto, int? excluirId, CancellationToken ct)
    {
        var ahora = _reloj.GetUtcNow().UtcDateTime;

        if (dto.FechaInicio <= ahora)
            throw new ValidacionNegocioException("La fecha de inicio debe ser futura.");

        if (dto.FechaFin <= dto.FechaInicio)
            throw new ValidacionNegocioException("La fecha de fin debe ser posterior a la de inicio.");

        if (await _conferencistaDA.ObtenerPorIdAsync(dto.ConferencistaId, ct) is null)
            throw new ValidacionNegocioException($"El conferencista con identificador '{dto.ConferencistaId}' no existe.");

        if (await _da.ExisteTraslapeAsync(dto.ConferencistaId, dto.FechaInicio, dto.FechaFin, excluirId, ct))
            throw new ConflictoException("El conferencista ya tiene otro evento en ese horario.");
    }

    private static bool TransicionValida(EstadoEvento actual, EstadoEvento nuevo) => (actual, nuevo) switch
    {
        (EstadoEvento.Programado, EstadoEvento.EnCurso) => true,
        (EstadoEvento.Programado, EstadoEvento.Cancelado) => true,
        (EstadoEvento.EnCurso, EstadoEvento.Finalizado) => true,
        (EstadoEvento.EnCurso, EstadoEvento.Cancelado) => true,
        _ => false
    };

    private static string? LimpiarOpcional(string? texto) =>
        string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();
}
