using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using StoreService.Domain.Entities;

namespace StoreService.Infrastructure.Data.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> b)
        {
            b.ToTable("Usuario");
            b.HasKey(x => x.Id);

            b.Property(x => x.Codigo)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn();
            b.HasIndex(x => x.Codigo).IsUnique();

            b.Property(x => x.Nome).IsRequired().HasMaxLength(150);
            b.Property(x => x.Email).IsRequired().HasMaxLength(255);
            b.HasIndex(x => x.Email).IsUnique();

            b.Property(x => x.SenhaHash).IsRequired();

            b.Property(x => x.CriadoEm).HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");
            b.Property(x => x.AtualizadoEm).HasColumnType("datetime2");
        }
    }
}