using Abstracciones.Conferencistas;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DA.Conferencistas;

public class ConferencistaConfiguracion : IEntityTypeConfiguration<Conferencista>
{
    public void Configure(EntityTypeBuilder<Conferencista> b)
    {
        b.ToTable("Conferencistas");
        b.HasKey(c => c.Id);

        b.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
        b.Property(c => c.Correo).IsRequired().HasMaxLength(150);
        b.Property(c => c.Especialidad).IsRequired().HasMaxLength(100);
        b.Property(c => c.Biografia).HasMaxLength(500);
        b.Property(c => c.FechaRegistro).HasDefaultValueSql("SYSUTCDATETIME()");

        // Índice único: red de seguridad contra correos duplicados aunque dos peticiones lleguen a la vez.
        b.HasIndex(c => c.Correo).IsUnique();
    }
}
