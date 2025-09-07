using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using StoreService.Domain.Entities;

namespace StoreService.Infrastructure.Data.Configurations
{
    public class MovimentacaoEstoqueConfiguration : IEntityTypeConfiguration<MovimentacaoEstoque>
    {
        public void Configure(EntityTypeBuilder<MovimentacaoEstoque> b)
        {
            b.ToTable("MovimentacaoEstoque", tb =>
            {
                tb.HasCheckConstraint("CK_Mov_Qtd", "[Quantidade] > 0");
                tb.HasCheckConstraint("CK_Mov_Tipo", "[Tipo] IN (0,1,2)");
            });

            b.HasKey(x => x.Id);

            b.Property(x => x.Codigo)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn();
            b.HasIndex(x => x.Codigo).IsUnique();

            b.Property(x => x.Quantidade).IsRequired();

            b.Property(x => x.NumeroNotaFiscal).HasMaxLength(50);
            b.Property(x => x.Observacao).HasMaxLength(250);

            b.Property(x => x.CriadoEm).HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            b.HasIndex(x => new { x.ProdutoId, x.CriadoEm });

            b.HasOne(x => x.Produto)
                .WithMany()
                .HasForeignKey(x => x.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.Pedido)
                .WithMany()
                .HasForeignKey(x => x.PedidoId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}