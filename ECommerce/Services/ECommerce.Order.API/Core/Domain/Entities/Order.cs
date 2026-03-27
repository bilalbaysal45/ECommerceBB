using ECommerce.Order.API.Core.Domain.Enums;

namespace ECommerce.Order.API.Core.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; private set; }
        public string UserId { get; private set; }
        public DateTime OrderDate { get; private set; } = DateTime.UtcNow;
        public decimal TotalPrice { get; private set; }
        public List<OrderItem> OrderItems { get; private set; }
        public OrderStatus Status { get; private set; }
        private Order() { }

        public Order(string userId, List<OrderItem> items)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            OrderItems = items;
            TotalPrice = items.Sum(x => x.UnitPrice * x.Quantity);
            Status = OrderStatus.Suspend; // Yeni sipariş her zaman askıda başlar
            OrderDate = DateTime.Now;
        }
        public void Complete()
        {
            if (Status == OrderStatus.Fail)
                throw new Exception("Başarısız bir sipariş tamamlanamaz!");

            Status = OrderStatus.Completed;
        }

        public void Reject()
        {
            Status = OrderStatus.Fail;
        }
    }
}
