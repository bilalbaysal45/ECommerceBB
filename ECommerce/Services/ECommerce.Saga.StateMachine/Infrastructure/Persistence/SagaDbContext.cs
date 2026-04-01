using MassTransit; // Kritik: SagaClassMap için
using Microsoft.EntityFrameworkCore;
using MassTransit.EntityFrameworkCoreIntegration;
using ECommerce.Saga.StateMachine.Core.Application.Sagas;
using ECommerce.Saga.StateMachine.Core.Domain.Entities;
namespace ECommerce.Saga.StateMachine.Infrastructure.Persistence
{
    public class SagaDbContext : DbContext
    {
        public SagaDbContext(DbContextOptions<SagaDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderState>(entity =>
            {
                entity.HasKey(x => x.CorrelationId);

                // Bire-Çok İlişki Tanımı
                entity.HasMany(x => x.OrderItems)
                      .WithOne()
                      .HasForeignKey(x => x.OrderStateId);
            });
            base.OnModelCreating(modelBuilder);

            // Extension metoduna güvenmek yerine doğrudan Map sınıfını kullanıyoruz
            var map = new OrderStateMap();
            map.Configure(modelBuilder);
        }
    }
}
