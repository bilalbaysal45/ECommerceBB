using ECommerce.Product.API.Core.Domain.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using DomainProduct = ECommerce.Product.API.Core.Domain.Entities.Product;

namespace ECommerce.Product.API.Infrastructure.Persistence
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options) : base(options) { }

        public DbSet<DomainProduct> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
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

                // İlişki yapılandırması Category
                entity.HasOne(p => p.Category)
                      .WithMany(c => c.Products)
                      .HasForeignKey(p => p.CategoryId)
                      .OnDelete(DeleteBehavior.Restrict); // Opsiyonel: Kategori silinince ürünler kalsın mı?
            });
            // İleride Category için de özel kurallar
            modelBuilder.Entity<Category>(entity => {
                entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
            });
            base.OnModelCreating(modelBuilder);

            modelBuilder.AddTransactionalOutboxEntities();
        }
    }
}
