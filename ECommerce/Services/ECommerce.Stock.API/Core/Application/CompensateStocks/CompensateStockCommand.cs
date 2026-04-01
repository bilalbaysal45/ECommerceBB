using ECommerce.Shared.Commons;
using MediatR;

namespace ECommerce.Stock.API.Core.Application.CompensateStocks
{
    public class CompensateStockCommand : IRequest<bool>
    {
        public Guid OrderId { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; }
    }
}
