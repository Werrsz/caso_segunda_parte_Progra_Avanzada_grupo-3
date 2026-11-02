using Abstracciones.Asistentes;
using Microsoft.AspNetCore.Mvc;

namespace GestorEventos.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AsistentesController : ControllerBase
{
    private readonly IAsistenteFlujo _flujo;

    public AsistentesController(IAsistenteFlujo flujo) => _flujo = flujo;

    [HttpGet]
    [ProducesResponseType<IReadOnlyList<AsistenteDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<AsistenteDto>>> Listar([FromQuery] AsistenteFiltro filtro, CancellationToken ct)
    {
        var resultado = await _flujo.ListarAsync(filtro, ct);
        Response.Headers["X-Total-Count"] = resultado.Total.ToString();
        Response.Headers["X-Page"] = resultado.Pagina.ToString();
        Response.Headers["X-Page-Size"] = resultado.Tamano.ToString();
        return Ok(resultado.Items);
    }

    [HttpGet("{id:int:min(1)}")]
    [ProducesResponseType<AsistenteDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AsistenteDto>> Obtener(int id, CancellationToken ct) =>
        Ok(await _flujo.ObtenerAsync(id, ct));

    [HttpPost]
    [ProducesResponseType<AsistenteDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<AsistenteDto>> Crear(AsistenteCrearDto dto, CancellationToken ct)
    {
        var creado = await _flujo.CrearAsync(dto, ct);
        return CreatedAtAction(nameof(Obtener), new { id = creado.Id }, creado);
    }

    [HttpPut("{id:int:min(1)}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Actualizar(int id, AsistenteActualizarDto dto, CancellationToken ct)
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
