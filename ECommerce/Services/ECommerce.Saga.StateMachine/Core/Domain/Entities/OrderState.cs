using MassTransit;

namespace ECommerce.Saga.StateMachine.Core.Domain.Entities
{
    public class OrderState : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; }

        // Süreç takibi için ek veriler
        public string UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public int Version { get; set; }
        public List<OrderStateItem> OrderItems { get; set; }
    }
}
