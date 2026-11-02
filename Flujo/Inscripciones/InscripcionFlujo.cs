using Abstracciones.Asistentes;
using Abstracciones.Comunes;
using Abstracciones.Eventos;
using Abstracciones.Excepciones;
using Abstracciones.Inscripciones;

namespace Flujo.Inscripciones;

public class InscripcionFlujo : IInscripcionFlujo
{
    private readonly IInscripcionDA _inscripcionDA;
    private readonly IEventoDA _eventoDA;
    private readonly IAsistenteDA _asistenteDA;
    private readonly TimeProvider _reloj;

    public InscripcionFlujo(
        IInscripcionDA inscripcionDA,
        IEventoDA eventoDA,
        IAsistenteDA asistenteDA,
        TimeProvider reloj)
    {
        _inscripcionDA = inscripcionDA;
        _eventoDA = eventoDA;
        _asistenteDA = asistenteDA;
        _reloj = reloj;
    }

    public async Task<ResultadoPaginado<InscripcionDto>> ListarAsync(InscripcionFiltro filtro, CancellationToken ct)
    {
        var pagina = await _inscripcionDA.BuscarAsync(filtro, ct);
        return new ResultadoPaginado<InscripcionDto>
        {
            Items = pagina.Items.Select(InscripcionMapeador.ADto).ToList(),
            Total = pagina.Total,
            Pagina = pagina.Pagina,
            Tamano = pagina.Tamano
        };
    }

    public async Task<InscripcionDto> ObtenerAsync(int id, CancellationToken ct)
    {
        var inscripcion = await _inscripcionDA.ObtenerPorIdAsync(id, ct)
            ?? throw new NoEncontradoException("Inscripcion", id);
        return InscripcionMapeador.ADto(inscripcion);
    }

    public async Task<InscripcionDto> CrearAsync(InscripcionCrearDto dto, CancellationToken ct)
    {
        var evento = await _eventoDA.ObtenerPorIdAsync(dto.EventoId, ct)
            ?? throw new ValidacionNegocioException($"El evento con identificador '{dto.EventoId}' no existe.");

        if (evento.Estado == EstadoEvento.Cancelado)
            throw new ConflictoException("No es posible inscribirse en un evento cancelado.");

        if (evento.Estado == EstadoEvento.Finalizado)
            throw new ConflictoException("No es posible inscribirse en un evento finalizado.");

        _ = await _asistenteDA.ObtenerPorIdAsync(dto.AsistenteId, ct)
            ?? throw new ValidacionNegocioException($"El asistente con identificador '{dto.AsistenteId}' no existe.");

        if (await _inscripcionDA.ExisteInscripcionAsync(dto.EventoId, dto.AsistenteId, ct))
            throw new ConflictoException("El asistente ya se encuentra inscrito en este evento.");

        var totalInscritos = await _inscripcionDA.ContarPorEventoAsync(dto.EventoId, ct);
        if (totalInscritos >= evento.CupoMaximo)
            throw new ConflictoException($"El evento ha alcanzado su cupo máximo de {evento.CupoMaximo} asistentes.");

        var inscripcion = new Inscripcion
        {
            EventoId = dto.EventoId,
            AsistenteId = dto.AsistenteId,
            FechaInscripcion = _reloj.GetUtcNow().UtcDateTime
        };

        await _inscripcionDA.AgregarAsync(inscripcion, ct);

        var creada = await _inscripcionDA.ObtenerPorIdAsync(inscripcion.Id, ct)
            ?? throw new NoEncontradoException("Inscripcion", inscripcion.Id);

        return InscripcionMapeador.ADto(creada);
    }

    public async Task EliminarAsync(int id, CancellationToken ct)
    {
        _ = await _inscripcionDA.ObtenerPorIdAsync(id, ct)
            ?? throw new NoEncontradoException("Inscripcion", id);

        if (!await _inscripcionDA.EliminarAsync(id, ct))
            throw new NoEncontradoException("Inscripcion", id);
    }
}
