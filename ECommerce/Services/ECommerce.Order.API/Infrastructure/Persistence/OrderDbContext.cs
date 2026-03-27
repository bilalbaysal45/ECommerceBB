using ECommerce.Order.API.Core.Domain.Entities;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using DomainOrder = ECommerce.Order.API.Core.Domain.Entities.Order;

namespace ECommerce.Order.API.Infrastructure.Persistence
{
    public class OrderDbContext :DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

        public DbSet<DomainOrder> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DomainOrder>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalPrice).HasPrecision(18, 2);
                // İlişki yapılandırması OrderItem
                entity.HasMany(p => p.OrderItems)
                      .WithOne(c => c.Order)
                      .HasForeignKey(oi => oi.OrderId)
                      .OnDelete(DeleteBehavior.Cascade); // Sipariş silinirse kalemleri de silinsin
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
            });

            base.OnModelCreating(modelBuilder);

            // MassTransit Outbox tablolarını (InboxState, OutboxState, OutboxMessage) ekler
            modelBuilder.AddTransactionalOutboxEntities();
        }
    }
}
