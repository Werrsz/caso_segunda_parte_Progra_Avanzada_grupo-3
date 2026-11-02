using Abstracciones.Asistentes;
using Abstracciones.Conferencistas;
using Abstracciones.Eventos;
using Abstracciones.Inscripciones;
using Microsoft.EntityFrameworkCore;

namespace DA.Contexto;

public class EventosContexto : DbContext
{
    public EventosContexto(DbContextOptions<EventosContexto> options) : base(options) { }

    public DbSet<Conferencista> Conferencistas => Set<Conferencista>();

    public DbSet<Evento> Eventos => Set<Evento>();

    public DbSet<Asistente> Asistentes => Set<Asistente>();

    public DbSet<Inscripcion> Inscripciones => Set<Inscripcion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Una clase de configuración por entidad, junto a su módulo (carpeta).
        // Esta línea las descubre todas: agregar un módulo no obliga a tocar este archivo.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EventosContexto).Assembly);
    }
}
