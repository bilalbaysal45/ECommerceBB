using ECommerce.Payment.API.Core.Application.Payments.Commands.ProcessPayment;
using ECommerce.Shared.Events.Payments;
using MassTransit;
using MediatR;

namespace ECommerce.Payment.API.Core.Application.Consumers
{
    public class ProcessPaymentCommandConsumer : IConsumer<ProcessPaymentCommand>
    {
        private readonly IMediator _mediator;

        public ProcessPaymentCommandConsumer(IMediator mediator) => _mediator = mediator;

        public async Task Consume(ConsumeContext<ProcessPaymentCommand> context)
        {
            var result = await _mediator.Send(new ProcessPaymentCommand
            {
                OrderId = context.Message.OrderId,
                TotalPrice = context.Message.TotalPrice
            });

            if (result)
                await context.Publish(new PaymentCompletedEvent { OrderId = context.Message.OrderId });
            else
                await context.Publish(new PaymentFailedEvent { OrderId = context.Message.OrderId, Message = "Bakiye yetersiz" });
        }
    }
}
