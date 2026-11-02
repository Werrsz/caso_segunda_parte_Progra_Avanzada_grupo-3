using Abstracciones.Inscripciones;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DA.Inscripciones;

public class InscripcionConfiguracion : IEntityTypeConfiguration<Inscripcion>
{
    public void Configure(EntityTypeBuilder<Inscripcion> b)
    {
        b.ToTable("Inscripciones");
        b.HasKey(i => i.Id);

        b.Property(i => i.EventoId).IsRequired();
        b.Property(i => i.AsistenteId).IsRequired();
        b.Property(i => i.FechaInscripcion).HasDefaultValueSql("SYSUTCDATETIME()");

        // Relación N-1 con Eventos. Restrict: no se borra un evento con inscripciones.
        b.HasOne(i => i.Evento)
            .WithMany(e => e.Inscripciones)
            .HasForeignKey(i => i.EventoId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación N-1 con Asistentes. Restrict: no se borra un asistente con inscripciones.
        b.HasOne(i => i.Asistente)
            .WithMany(a => a.Inscripciones)
            .HasForeignKey(i => i.AsistenteId)
            .OnDelete(DeleteBehavior.Restrict);

        // Regla en BD: no inscribirse dos veces al mismo evento (índice único compuesto EventoId + AsistenteId).
        b.HasIndex(i => new { i.EventoId, i.AsistenteId }).IsUnique();
        b.HasIndex(i => i.EventoId);
        b.HasIndex(i => i.AsistenteId);
    }
}
