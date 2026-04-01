namespace ECommerce.Saga.StateMachine.Core.Domain.Entities
{
    public class OrderStateItem
    {
        public Guid Id { get; set; }
        public Guid OrderStateId { get; set; } // Foreign Key
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
