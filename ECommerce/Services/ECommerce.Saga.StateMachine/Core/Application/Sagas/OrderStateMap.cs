using ECommerce.Shared.Sagas;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Saga.StateMachine.Core.Application.Sagas
{
    public class OrderStateMap : SagaClassMap<OrderState>
    {
        protected override void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<OrderState> entity, ModelBuilder model)
        {
            // Primary Key
            entity.HasKey(x => x.CorrelationId);

            entity.Property(x => x.CurrentState).HasMaxLength(64);
            entity.Property(x => x.UserId).HasMaxLength(128);

            entity.Property(x => x.TotalPrice).HasColumnType("decimal(18,2)");
            entity.Property(x => x.Version).IsConcurrencyToken();
        }
    }
}
