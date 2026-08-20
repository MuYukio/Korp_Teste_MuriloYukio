using Faturamento.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Faturamento.Infrastructure.Persistence;

public class FaturamentoDbContext : DbContext
{
    public FaturamentoDbContext(DbContextOptions<FaturamentoDbContext> options) : base(options)
    {
    }

    public DbSet<NotaFiscal> NotasFiscais => Set<NotaFiscal>();
    public DbSet<ItemNotaFiscal> ItensNotaFiscal => Set<ItemNotaFiscal>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<NotaFiscal>(entity =>
        {
            entity.HasKey(n => n.Id);

            entity.Property(n => n.Numero).IsRequired();
            entity.HasIndex(n => n.Numero).IsUnique();

            entity.Property(n => n.Status)
                  .HasConversion<string>()
                  .IsRequired();

            // Mapeia o campo privado _itens como a coleção de navegação
            entity.HasMany(n => n.Itens)
                  .WithOne()
                  .HasForeignKey("NotaFiscalId")
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Metadata
                  .FindNavigation(nameof(NotaFiscal.Itens))!
                  .SetPropertyAccessMode(Microsoft.EntityFrameworkCore.PropertyAccessMode.Field);
        });

        modelBuilder.Entity<ItemNotaFiscal>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.ProdutoCodigo).IsRequired();
            entity.Property(i => i.ProdutoDescricao).IsRequired();
            entity.Property(i => i.Quantidade).IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}