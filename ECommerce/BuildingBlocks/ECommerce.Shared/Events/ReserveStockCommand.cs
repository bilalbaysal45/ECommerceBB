using ECommerce.Shared.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.Events
{
    public class ReserveStockCommand
    {
        public Guid OrderId { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; }
    }
}
