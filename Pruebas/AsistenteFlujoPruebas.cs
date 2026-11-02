using Abstracciones.Asistentes;
using Abstracciones.Comunes;
using Abstracciones.Excepciones;
using FluentAssertions;
using Flujo.Asistentes;
using Moq;
using Xunit;

namespace Pruebas;

public class AsistenteFlujoPruebas
{
    private readonly Mock<IAsistenteDA> _asistenteDaMock = new();
    private readonly TimeProvider _reloj = TimeProvider.System;
    private readonly AsistenteFlujo _flujo;

    public AsistenteFlujoPruebas()
    {
        _flujo = new AsistenteFlujo(_asistenteDaMock.Object, _reloj);
    }

    [Fact]
    public async Task CrearAsync_DatosValidos_CreaYRetornaDto()
    {
        // Arrange
        var dto = new AsistenteCrearDto
        {
            Nombre = "Juan Perez",
            Correo = "juan.perez@example.com"
        };

        _asistenteDaMock
            .Setup(a => a.ObtenerPorCorreoAsync("juan.perez@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Asistente?)null);

        _asistenteDaMock
            .Setup(a => a.AgregarAsync(It.IsAny<Asistente>(), It.IsAny<CancellationToken>()))
            .Callback<Asistente, CancellationToken>((a, _) => a.Id = 42)
            .ReturnsAsync((Asistente a, CancellationToken _) => a);

        // Act
        var resultado = await _flujo.CrearAsync(dto, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(42);
        resultado.Nombre.Should().Be("Juan Perez");
        resultado.Correo.Should().Be("juan.perez@example.com");
    }

    [Fact]
    public async Task CrearAsync_CorreoDuplicado_LanzaConflictoException()
    {
        // Arrange
        var dto = new AsistenteCrearDto
        {
            Nombre = "Juan Perez",
            Correo = "juan.perez@example.com"
        };

        _asistenteDaMock
            .Setup(a => a.ObtenerPorCorreoAsync("juan.perez@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Asistente { Id = 1, Correo = "juan.perez@example.com" });

        // Act
        var accion = () => _flujo.CrearAsync(dto, CancellationToken.None);

        // Assert
        await accion.Should().ThrowAsync<ConflictoException>()
            .WithMessage("*Ya existe un asistente con el correo*");
    }

    [Fact]
    public async Task ObtenerAsync_Existente_RetornaDto()
    {
        // Arrange
        var existente = new Asistente
        {
            Id = 10,
            Nombre = "Maria Lopez",
            Correo = "maria.lopez@example.com",
            FechaRegistro = DateTime.UtcNow
        };

        _asistenteDaMock
            .Setup(a => a.ObtenerPorIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existente);

        // Act
        var resultado = await _flujo.ObtenerAsync(10, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(10);
        resultado.Nombre.Should().Be("Maria Lopez");
        resultado.Correo.Should().Be("maria.lopez@example.com");
    }

    [Fact]
    public async Task ObtenerAsync_NoExiste_LanzaNoEncontradoException()
    {
        // Arrange
        _asistenteDaMock
            .Setup(a => a.ObtenerPorIdAsync(123, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Asistente?)null);

        // Act
        var accion = () => _flujo.ObtenerAsync(123, CancellationToken.None);

        // Assert
        await accion.Should().ThrowAsync<NoEncontradoException>();
    }

    [Fact]
    public async Task ActualizarAsync_DatosValidos_ActualizaCorrectamente()
    {
        // Arrange
        var asistente = new Asistente { Id = 1, Nombre = "Juan", Correo = "juan@example.com" };
        var dto = new AsistenteActualizarDto { Nombre = "Juan Modificado", Correo = "juan.nuevo@example.com" };

        _asistenteDaMock
            .Setup(a => a.ObtenerPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(asistente);

        _asistenteDaMock
            .Setup(a => a.ObtenerPorCorreoAsync("juan.nuevo@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Asistente?)null);

        _asistenteDaMock
            .Setup(a => a.ActualizarAsync(It.IsAny<Asistente>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var accion = () => _flujo.ActualizarAsync(1, dto, CancellationToken.None);

        // Assert
        await accion.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ActualizarAsync_CorreoEnUsoPorOtro_LanzaConflictoException()
    {
        // Arrange
        var asistente = new Asistente { Id = 1, Nombre = "Juan", Correo = "juan@example.com" };
        var dto = new AsistenteActualizarDto { Nombre = "Juan Modificado", Correo = "otro@example.com" };

        _asistenteDaMock
            .Setup(a => a.ObtenerPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(asistente);

        _asistenteDaMock
            .Setup(a => a.ObtenerPorCorreoAsync("otro@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Asistente { Id = 2, Correo = "otro@example.com" });

        // Act
        var accion = () => _flujo.ActualizarAsync(1, dto, CancellationToken.None);

        // Assert
        await accion.Should().ThrowAsync<ConflictoException>()
            .WithMessage("*Ya existe otro asistente*");
    }

    [Fact]
    public async Task EliminarAsync_NoExiste_LanzaNoEncontradoException()
    {
        // Arrange
        _asistenteDaMock
            .Setup(a => a.ObtenerPorIdAsync(55, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Asistente?)null);

        // Act
        var accion = () => _flujo.EliminarAsync(55, CancellationToken.None);

        // Assert
        await accion.Should().ThrowAsync<NoEncontradoException>();
    }

    [Fact]
    public async Task ListarAsync_RetornaResultadoPaginado()
    {
        // Arrange
        var filtro = new AsistenteFiltro { Pagina = 1, Tamano = 10 };
        var lista = new List<Asistente>
        {
            new() { Id = 1, Nombre = "Ana", Correo = "ana@example.com", FechaRegistro = DateTime.UtcNow },
            new() { Id = 2, Nombre = "Luis", Correo = "luis@example.com", FechaRegistro = DateTime.UtcNow }
        };

        _asistenteDaMock
            .Setup(a => a.BuscarAsync(filtro, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResultadoPaginado<Asistente>
            {
                Items = lista,
                Total = 2,
                Pagina = 1,
                Tamano = 10
            });

        // Act
        var resultado = await _flujo.ListarAsync(filtro, CancellationToken.None);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Total.Should().Be(2);
        resultado.Items.Should().HaveCount(2);
        resultado.Items[0].Nombre.Should().Be("Ana");
    }
}
