namespace Abstracciones.Conferencistas;

public class Conferencista
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Correo { get; set; } = string.Empty;
    public string Especialidad { get; set; } = string.Empty;
    public string? Biografia { get; set; }
    public DateTime FechaRegistro { get; set; }
}
