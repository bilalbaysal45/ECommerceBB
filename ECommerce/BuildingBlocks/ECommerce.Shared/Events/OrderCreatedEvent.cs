using ECommerce.Shared.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.Events
{
    public class OrderCreatedEvent
    {
        public Guid OrderId { get; set; }
        public string UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public List<OrderItemMessage> Items { get; set; }
    }
}
