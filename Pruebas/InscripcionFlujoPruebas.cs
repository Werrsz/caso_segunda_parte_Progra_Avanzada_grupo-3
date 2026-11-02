using Abstracciones.Asistentes;
using Abstracciones.Comunes;
using Abstracciones.Eventos;
using Abstracciones.Excepciones;
using Abstracciones.Inscripciones;
using FluentAssertions;
using Flujo.Inscripciones;
using Moq;
using Xunit;

namespace Pruebas;

public class InscripcionFlujoPruebas
{
    private readonly Mock<IInscripcionDA> _inscripcionDaMock = new();
    private readonly Mock<IEventoDA> _eventoDaMock = new();
    private readonly Mock<IAsistenteDA> _asistenteDaMock = new();
    private readonly TimeProvider _reloj = TimeProvider.System;
    private readonly InscripcionFlujo _flujo;

    public InscripcionFlujoPruebas()
    {
        _flujo = new InscripcionFlujo(
            _inscripcionDaMock.Object,
            _eventoDaMock.Object,
            _asistenteDaMock.Object,
            _reloj);
    }

    [Fact]
    public async Task CrearAsync_DatosValidos_CreaYRetornaDto()
    {
        // Arrange
        var dto = new InscripcionCrearDto { EventoId = 1, AsistenteId = 10 };
        var evento = new Evento { Id = 1, Titulo = "Conferencia IA", CupoMaximo = 50, Estado = EstadoEvento.Programado };
        var asistente = new Asistente { Id = 10, Nombre = "Ada Lovelace", Correo = "ada@example.com" };

        _eventoDaMock
            .Setup(e => e.ObtenerPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(evento);

        _asistenteDaMock
            .Setup(a => a.ObtenerPorIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(asistente);

        _inscripcionDaMock
            .Setup(i => i.ExisteInscripcionAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _inscripcionDaMock
            .Setup(i => i.ContarPorEventoAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        _inscripcionDaMock
            .Setup(i => i.AgregarAsync(It.IsAny<Inscripcion>(), It.IsAny<CancellationToken>()))
            .Callback<Inscripcion, CancellationToken>((ins, _) => ins.Id = 99)
            .ReturnsAsync((Inscripcion ins, CancellationToken _) => ins);

        _inscripcionDaMock
            .Setup(i => i.ObtenerPorIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Inscripcion
            {
                Id = 99,
                EventoId = 1,
                Evento = evento,
                AsistenteId = 10,
                Asistente = asistente,
                FechaInscripcion = DateTime.UtcNow
            });

        // Act
        var resultado = await _flujo.CrearAsync(dto, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(99);
        resultado.EventoId.Should().Be(1);
        resultado.EventoTitulo.Should().Be("Conferencia IA");
        resultado.AsistenteId.Should().Be(10);
        resultado.AsistenteNombre.Should().Be("Ada Lovelace");
    }

    [Fact]
    public async Task CrearAsync_EventoNoExiste_LanzaValidacionNegocioException()
    {
        // Arrange
        var dto = new InscripcionCrearDto { EventoId = 999, AsistenteId = 10 };

        _eventoDaMock
            .Setup(e => e.ObtenerPorIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Evento?)null);

        // Act
        var accion = () => _flujo.CrearAsync(dto, CancellationToken.None);

        // Assert
        await accion.Should().ThrowAsync<ValidacionNegocioException>()
            .WithMessage("*identificador '999' no existe*");
    }

    [Fact]
    public async Task CrearAsync_EventoCancelado_LanzaConflictoException()
    {
        // Arrange
        var dto = new InscripcionCrearDto { EventoId = 1, AsistenteId = 10 };
        var evento = new Evento { Id = 1, Titulo = "Evento Cancelado", CupoMaximo = 50, Estado = EstadoEvento.Cancelado };

        _eventoDaMock
            .Setup(e => e.ObtenerPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(evento);

        // Act
        var accion = () => _flujo.CrearAsync(dto, CancellationToken.None);

        // Assert
        await accion.Should().ThrowAsync<ConflictoException>()
            .WithMessage("*cancelado*");
    }

    [Fact]
    public async Task CrearAsync_EventoFinalizado_LanzaConflictoException()
    {
        // Arrange
        var dto = new InscripcionCrearDto { EventoId = 1, AsistenteId = 10 };
        var evento = new Evento { Id = 1, Titulo = "Evento Finalizado", CupoMaximo = 50, Estado = EstadoEvento.Finalizado };

        _eventoDaMock
            .Setup(e => e.ObtenerPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(evento);

        // Act
        var accion = () => _flujo.CrearAsync(dto, CancellationToken.None);

        // Assert
        await accion.Should().ThrowAsync<ConflictoException>()
            .WithMessage("*finalizado*");
    }

    [Fact]
    public async Task CrearAsync_AsistenteNoExiste_LanzaValidacionNegocioException()
    {
        // Arrange
        var dto = new InscripcionCrearDto { EventoId = 1, AsistenteId = 999 };
        var evento = new Evento { Id = 1, Titulo = "Evento", CupoMaximo = 50, Estado = EstadoEvento.Programado };

        _eventoDaMock
            .Setup(e => e.ObtenerPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(evento);

        _asistenteDaMock
            .Setup(a => a.ObtenerPorIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Asistente?)null);

        // Act
        var accion = () => _flujo.CrearAsync(dto, CancellationToken.None);

        // Assert
        await accion.Should().ThrowAsync<ValidacionNegocioException>()
            .WithMessage("*identificador '999' no existe*");
    }

    [Fact]
    public async Task CrearAsync_AsistenteYaInscrito_LanzaConflictoException()
    {
        // Arrange
        var dto = new InscripcionCrearDto { EventoId = 1, AsistenteId = 10 };
        var evento = new Evento { Id = 1, Titulo = "Evento", CupoMaximo = 50, Estado = EstadoEvento.Programado };
        var asistente = new Asistente { Id = 10, Nombre = "Ada", Correo = "ada@example.com" };

        _eventoDaMock
            .Setup(e => e.ObtenerPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(evento);

        _asistenteDaMock
            .Setup(a => a.ObtenerPorIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(asistente);

        _inscripcionDaMock
            .Setup(i => i.ExisteInscripcionAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var accion = () => _flujo.CrearAsync(dto, CancellationToken.None);

        // Assert
        await accion.Should().ThrowAsync<ConflictoException>()
            .WithMessage("*ya se encuentra inscrito*");
    }

    [Fact]
    public async Task CrearAsync_CupoMaximoAlcanzado_LanzaConflictoException()
    {
        // Arrange
        var dto = new InscripcionCrearDto { EventoId = 1, AsistenteId = 10 };
        var evento = new Evento { Id = 1, Titulo = "Evento Lleno", CupoMaximo = 20, Estado = EstadoEvento.Programado };
        var asistente = new Asistente { Id = 10, Nombre = "Ada", Correo = "ada@example.com" };

        _eventoDaMock
            .Setup(e => e.ObtenerPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(evento);

        _asistenteDaMock
            .Setup(a => a.ObtenerPorIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(asistente);

        _inscripcionDaMock
            .Setup(i => i.ExisteInscripcionAsync(1, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _inscripcionDaMock
            .Setup(i => i.ContarPorEventoAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(20);

        // Act
        var accion = () => _flujo.CrearAsync(dto, CancellationToken.None);

        // Assert
        await accion.Should().ThrowAsync<ConflictoException>()
            .WithMessage("*cupo máximo*");
    }

    [Fact]
    public async Task ObtenerAsync_NoExiste_LanzaNoEncontradoException()
    {
        // Arrange
        _inscripcionDaMock
            .Setup(i => i.ObtenerPorIdAsync(123, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Inscripcion?)null);

        // Act
        var accion = () => _flujo.ObtenerAsync(123, CancellationToken.None);

        // Assert
        await accion.Should().ThrowAsync<NoEncontradoException>();
    }

    [Fact]
    public async Task EliminarAsync_NoExiste_LanzaNoEncontradoException()
    {
        // Arrange
        _inscripcionDaMock
            .Setup(i => i.ObtenerPorIdAsync(55, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Inscripcion?)null);

        // Act
        var accion = () => _flujo.EliminarAsync(55, CancellationToken.None);

        // Assert
        await accion.Should().ThrowAsync<NoEncontradoException>();
    }

    [Fact]
    public async Task ListarAsync_RetornaResultadoPaginado()
    {
        // Arrange
        var filtro = new InscripcionFiltro { Pagina = 1, Tamano = 10, EventoId = 1 };
        var evento = new Evento { Id = 1, Titulo = "Evento 1" };
        var asistente = new Asistente { Id = 2, Nombre = "Ada", Correo = "ada@example.com" };

        var lista = new List<Inscripcion>
        {
            new() { Id = 1, EventoId = 1, Evento = evento, AsistenteId = 2, Asistente = asistente, FechaInscripcion = DateTime.UtcNow }
        };

        _inscripcionDaMock
            .Setup(i => i.BuscarAsync(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResultadoPaginado<Inscripcion>
            {
                Items = lista,
                Total = 1,
                Pagina = 1,
                Tamano = 10
            });

        // Act
        var resultado = await _flujo.ListarAsync(filtro, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Total.Should().Be(1);
        resultado.Items.Should().HaveCount(1);
        resultado.Items[0].EventoTitulo.Should().Be("Evento 1");
        resultado.Items[0].AsistenteNombre.Should().Be("Ada");
    }
}
