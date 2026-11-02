using Abstracciones.Asistentes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DA.Asistentes;

public class AsistenteConfiguracion : IEntityTypeConfiguration<Asistente>
{
    public void Configure(EntityTypeBuilder<Asistente> b)
    {
        b.ToTable("Asistentes");
        b.HasKey(a => a.Id);

        b.Property(a => a.Nombre).IsRequired().HasMaxLength(100);
        b.Property(a => a.Correo).IsRequired().HasMaxLength(150);
        b.Property(a => a.FechaRegistro).HasDefaultValueSql("SYSUTCDATETIME()");

        // Índice único en Correo para garantizar que cada asistente sea único en el catálogo
        b.HasIndex(a => a.Correo).IsUnique();
    }
}
