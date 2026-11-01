namespace Abstracciones.Excepciones;

/// <summary>
/// Base de los errores que Flujo y DA comunican hacia arriba SIN depender de HTTP.
/// La capa API los traduce a códigos HTTP en un solo lugar (ManejadorExcepcionesGlobal).
/// </summary>
public abstract class ExcepcionDeNegocio : Exception
{
    protected ExcepcionDeNegocio(string mensaje) : base(mensaje) { }
}

/// <summary>El recurso pedido no existe. HTTP 404.</summary>
public class NoEncontradoException : ExcepcionDeNegocio
{
    public NoEncontradoException(string entidad, object id)
        : base($"{entidad} con identificador '{id}' no existe.") { }
}

/// <summary>La operación choca con el estado actual (duplicado, transición inválida, regla). HTTP 409.</summary>
public class ConflictoException : ExcepcionDeNegocio
{
    public ConflictoException(string mensaje) : base(mensaje) { }
}

/// <summary>El cuerpo es válido en forma, pero referencia algo inexistente o rompe una regla. HTTP 422.</summary>
public class ValidacionNegocioException : ExcepcionDeNegocio
{
    public ValidacionNegocioException(string mensaje) : base(mensaje) { }
}
