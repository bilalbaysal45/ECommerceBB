using Microsoft.EntityFrameworkCore;
using DomainProduct = ECommerce.Product.API.Core.Domain.Entities.Product;

namespace ECommerce.Product.API.Infrastructure.Persistence
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) { }

        public DbSet<DomainProduct> Products { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Veritabanı yapılandırması (Fluent API)
            modelBuilder.Entity<DomainProduct>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Sku).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Sku).IsUnique(); // SKU eşsiz olmalı
                entity.Property(e => e.Price).HasPrecision(18, 2);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
