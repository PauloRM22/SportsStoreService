using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using StoreService.Domain.Entities;

namespace StoreService.Infrastructure.Data.Configurations
{
    public class ProdutoConfiguration : IEntityTypeConfiguration<Produto>
    {
        public void Configure(EntityTypeBuilder<Produto> b)
        {
            b.ToTable("Produto", tb =>
            {
                tb.HasCheckConstraint("CK_Produto_Preco", "[Preco] >= 0");
            });

            b.HasKey(x => x.Id);

            b.Property(x => x.Codigo)
                .ValueGeneratedOnAdd()
                .UseIdentityColumn();
            b.HasIndex(x => x.Codigo).IsUnique();

            b.Property(x => x.Nome).IsRequired().HasMaxLength(150);
            b.Property(x => x.Descricao).HasMaxLength(1000);

            b.Property(x => x.Preco).IsRequired().HasColumnType("decimal(18,2)");

            b.Property(x => x.Ativo).HasDefaultValue(true);

            b.Property(x => x.CriadoEm).HasColumnType("datetime2")
                .HasDefaultValueSql("SYSUTCDATETIME()");
            b.Property(x => x.AtualizadoEm).HasColumnType("datetime2");
        }
    }
}