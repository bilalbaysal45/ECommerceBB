using MassTransit;
using Microsoft.EntityFrameworkCore;
using DomainStock = ECommerce.Stock.API.Core.Domain.Entities.Stock;

namespace ECommerce.Stock.API.Infrastructure.Persistence
{
    public class StockDbContext : DbContext
    {
        public StockDbContext(DbContextOptions<StockDbContext> options) : base(options) { }

        public DbSet<DomainStock> Stocks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DomainStock>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.ProductId).IsUnique();
                entity.Property(e => e.ProductId).IsRequired();
                entity.Property(e => e.StockCount).IsRequired();
            });

            base.OnModelCreating(modelBuilder);

            modelBuilder.AddTransactionalOutboxEntities();
        }
    }
}
