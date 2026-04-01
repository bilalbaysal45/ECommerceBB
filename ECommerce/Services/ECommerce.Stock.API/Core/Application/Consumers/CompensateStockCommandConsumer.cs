using ECommerce.Stock.API.Core.Application.CompensateStocks;
using MassTransit;
using MediatR;

namespace ECommerce.Stock.API.Core.Application.Consumers
{
    public class CompensateStockCommandConsumer : IConsumer<CompensateStockCommand>
    {
        private readonly IMediator _mediator;
        public CompensateStockCommandConsumer(IMediator mediator) => _mediator = mediator;

        public async Task Consume(ConsumeContext<CompensateStockCommand> context)
        {
            await _mediator.Send(new CompensateStockCommand
            {
                OrderId = context.Message.OrderId,
                OrderItems = context.Message.OrderItems
            });
        }
    }
}
