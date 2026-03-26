using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.Events
{
    public class StockNotEnoughEvent
    {
        public Guid OrderId { get; set; }
        public string Message { get; set; }
    }
}
