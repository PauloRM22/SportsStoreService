using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using StoreService.Domain.Entities;

namespace StoreService.Infrastructure.Data.Configurations
{
    public class ProdutoSaldoConfiguration : IEntityTypeConfiguration<ProdutoSaldo>
    {
        public void Configure(EntityTypeBuilder<ProdutoSaldo> b)
        {
            b.ToTable("ProdutoSaldo");
            b.HasKey(x => x.ProdutoId);

            b.Property(x => x.QuantidadeAtual).IsRequired();
            b.Property(x => x.AtualizadoEm).HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");

            b.HasOne(x => x.Produto)
                .WithOne()
                .HasForeignKey<ProdutoSaldo>(x => x.ProdutoId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}