using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using StoreService.Domain.Entities;
using StoreService.Domain.Enums;

namespace StoreService.Infrastructure.Data.Configurations
{
    public class PedidoConfiguration : IEntityTypeConfiguration<Pedido>
    {
        public void Configure(EntityTypeBuilder<Pedido> b)
        {
            b.ToTable("Pedido", tb =>
            {
                tb.HasCheckConstraint("CK_Pedido_Total", "[ValorTotal] >= 0");
            });

            b.HasKey(x => x.Id);

            b.Property(x => x.Codigo)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn();
            b.HasIndex(x => x.Codigo).IsUnique();

            b.Property(x => x.ClienteDocumento).IsRequired().HasMaxLength(50);
            b.Property(x => x.VendedorNome).IsRequired().HasMaxLength(150);

            b.Property(x => x.ValorTotal).HasColumnType("decimal(18,2)").HasDefaultValue(0);

            b.Property(x => x.Status).HasDefaultValue(StatusPedido.Rascunho);

            b.Property(x => x.CriadoEm).HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");
            b.Property(x => x.AtualizadoEm).HasColumnType("datetime2");

            b.HasOne(x => x.Vendedor)
                .WithMany()
                .HasForeignKey(x => x.VendedorId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => x.CriadoEm);
        }
    }
}