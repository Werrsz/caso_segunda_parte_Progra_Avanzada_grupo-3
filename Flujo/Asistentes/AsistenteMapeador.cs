using Abstracciones.Asistentes;

namespace Flujo.Asistentes;

public static class AsistenteMapeador
{
    public static AsistenteDto ADto(Asistente a) =>
        new(a.Id, a.Nombre, a.Correo, a.FechaRegistro);
}
