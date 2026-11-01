using Abstracciones.Conferencistas;
using Microsoft.AspNetCore.Mvc;

namespace GestorEventos.API.Controllers;

/// <summary>
/// El controlador recibe, delega a Flujo y traduce el resultado. Sin try/catch ni lógica de negocio:
/// los errores los traduce el ManejadorExcepcionesGlobal.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ConferencistasController : ControllerBase
{
    private readonly IConferencistaFlujo _flujo;

    public ConferencistasController(IConferencistaFlujo flujo) => _flujo = flujo;

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<ConferencistaDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<ConferencistaDto>>> Listar([FromQuery] ConferencistaFiltro filtro, CancellationToken ct)
    {
        var resultado = await _flujo.ListarAsync(filtro, ct);
        Response.Headers["X-Total-Count"] = resultado.Total.ToString();
        Response.Headers["X-Page"] = resultado.Pagina.ToString();
        Response.Headers["X-Page-Size"] = resultado.Tamano.ToString();
        return Ok(resultado.Items);
    }

    [HttpGet("{id:int:min(1)}")]
    [ProducesResponseType<ConferencistaDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConferencistaDto>> Obtener(int id, CancellationToken ct) =>
        Ok(await _flujo.ObtenerAsync(id, ct));

    [HttpPost]
    [ProducesResponseType<ConferencistaDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ConferencistaDto>> Crear(ConferencistaCrearDto dto, CancellationToken ct)
    {
        var creado = await _flujo.CrearAsync(dto, ct);
        return CreatedAtAction(nameof(Obtener), new { id = creado.Id }, creado);
    }

    [HttpPut("{id:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Actualizar(int id, ConferencistaActualizarDto dto, CancellationToken ct)
    {
        await _flujo.ActualizarAsync(id, dto, ct);
        return NoContent();
    }

    [HttpDelete("{id:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Eliminar(int id, CancellationToken ct)
    {
        await _flujo.EliminarAsync(id, ct);
        return NoContent();
    }
}
