using Abstracciones.Conferencistas;
using Microsoft.EntityFrameworkCore;

namespace DA.Contexto;

public class EventosContexto : DbContext
{
    public EventosContexto(DbContextOptions<EventosContexto> options) : base(options) { }

    public DbSet<Conferencista> Conferencistas => Set<Conferencista>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Una clase de configuración por entidad, junto a su módulo (carpeta).
        // Esta línea las descubre todas: agregar un módulo no obliga a tocar este archivo.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EventosContexto).Assembly);
    }
}
