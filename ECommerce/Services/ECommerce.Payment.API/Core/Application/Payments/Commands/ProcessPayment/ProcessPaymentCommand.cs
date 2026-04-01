using MediatR;

namespace ECommerce.Payment.API.Core.Application.Payments.Commands.ProcessPayment
{
    public class ProcessPaymentCommand : IRequest<bool>
    {
        public Guid OrderId { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
