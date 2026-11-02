using Abstracciones.Eventos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DA.Eventos;

public class EventoConfiguracion : IEntityTypeConfiguration<Evento>
{
    public void Configure(EntityTypeBuilder<Evento> b)
    {
        b.ToTable("Eventos");
        b.HasKey(e => e.Id);

        b.Property(e => e.Titulo).IsRequired().HasMaxLength(150);
        b.Property(e => e.Descripcion).HasMaxLength(1000);
        b.Property(e => e.Lugar).IsRequired().HasMaxLength(150);
        b.Property(e => e.CupoMaximo).IsRequired();
        b.Property(e => e.Estado).HasConversion<string>().HasMaxLength(20).IsRequired();
        b.Property(e => e.FechaCreacion).HasDefaultValueSql("SYSUTCDATETIME()");

        // Relación 1-N Conferencista -> Evento. Restrict: no se borra un conferencista con eventos.
        b.HasOne(e => e.Conferencista)
            .WithMany(c => c.Eventos)
            .HasForeignKey(e => e.ConferencistaId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(e => e.ConferencistaId);
        b.HasIndex(e => e.FechaInicio);
        b.HasIndex(e => e.Estado);
    }
}
