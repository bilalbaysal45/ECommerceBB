using ECommerce.Payment.API.Core.Application.Payments.Commands.ProcessPayment;
using ECommerce.Payment.API.Infrastructure.Persistence;
using ECommerce.Shared.Events.Payments;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace ECommerce.Payment.API.Core.Application.Consumers
{
    public class ProcessPaymentCommandConsumer : IConsumer<ProcessPaymentCommand>
    {
        private readonly IMediator _mediator;
        private readonly PaymentDbContext _context;
        public ProcessPaymentCommandConsumer(IMediator mediator, PaymentDbContext context)
        {
            _mediator = mediator;
            _context = context;
        }

        public async Task Consume(ConsumeContext<ProcessPaymentCommand> context)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
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
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
