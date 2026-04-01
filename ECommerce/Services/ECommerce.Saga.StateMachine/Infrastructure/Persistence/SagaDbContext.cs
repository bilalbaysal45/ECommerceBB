using MassTransit; // Kritik: SagaClassMap için
using Microsoft.EntityFrameworkCore; // ModelBuilder için
using ECommerce.Shared.Sagas;
using MassTransit.EntityFrameworkCoreIntegration;
using ECommerce.Saga.StateMachine.Core.Application.Sagas; // Extension metodlar için
namespace ECommerce.Saga.StateMachine.Infrastructure.Persistence
{
    public class SagaDbContext : DbContext
    {
        public SagaDbContext(DbContextOptions<SagaDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Extension metoduna güvenmek yerine doğrudan Map sınıfını kullanıyoruz
            var map = new OrderStateMap();
            map.Configure(modelBuilder);
        }
    }
}
