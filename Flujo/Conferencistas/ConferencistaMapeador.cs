using Abstracciones.Conferencistas;

namespace Flujo.Conferencistas;

public static class ConferencistaMapeador
{
    public static ConferencistaDto ADto(Conferencista c) =>
        new(c.Id, c.Nombre, c.Correo, c.Especialidad, c.Biografia, c.FechaRegistro);
}
