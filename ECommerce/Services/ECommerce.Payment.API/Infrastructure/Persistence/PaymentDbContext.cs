using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Payment.API.Infrastructure.Persistence
{
    public class PaymentDbContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();

        }
    }
}
