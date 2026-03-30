using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Shared.Events
{
    public record ProductCreatedEvent
    {
        public Guid ProductId { get; init; }
        public string SKU { get; init; }
        public string ProductName { get; init; }
    }
}
