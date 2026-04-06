using ECommerce.Order.API.Core.Domain.Entities;
using ECommerce.Order.API.Core.Domain.Enums;

namespace ECommerce.Order.API.Models.Dtos
{
    public class OrderDto
    {
        public Guid Id { get; private set; }
        public string UserId { get; private set; }
        public DateTime OrderDate { get; private set; }
        public decimal TotalPrice { get; private set; }
        public OrderStatus Status { get; private set; }
        public List<OrderItem> OrderItems { get; private set; }
    }
}
