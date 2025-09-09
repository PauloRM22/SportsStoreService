using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using StoreService.Domain.Entities;

namespace StoreService.Infrastructure.Data.Configurations
{
    public class PedidoItemConfiguration : IEntityTypeConfiguration<PedidoItem>
    {
        public void Configure(EntityTypeBuilder<PedidoItem> b)
        {
            b.ToTable("PedidoItem", tb =>
            {
                tb.HasCheckConstraint("CK_PedidoItem_Qtd", "[Quantidade] > 0");
                tb.HasCheckConstraint("CK_PedidoItem_Preco", "[PrecoUnitario] >= 0");
            });

            b.HasKey(x => x.Id);

            b.Property(x => x.Codigo)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn();
            b.HasIndex(x => x.Codigo).IsUnique();

            b.Property(x => x.Quantidade).IsRequired();

            b.Property(x => x.PrecoUnitario).HasColumnType("decimal(18,2)").IsRequired();

            b.Property(x => x.Subtotal)
                .HasColumnType("decimal(18,2)")
                .HasComputedColumnSql("[Quantidade] * [PrecoUnitario]", stored: true);

            b.HasIndex(x => x.PedidoId);

            b.HasOne(x => x.Pedido)
                .WithMany(p => p.Itens)
                .HasForeignKey(x => x.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasOne(x => x.Produto)
                .WithMany()
                .HasForeignKey(x => x.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Um produto por pedido (não repetir)
            b.HasIndex(x => new { x.PedidoId, x.ProdutoId }).IsUnique();
        }
    }
}