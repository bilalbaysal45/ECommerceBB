using MediatR;

namespace ECommerce.Payment.API.Core.Application.Payments.Commands.ProcessPayment
{
    public class ProcessPaymentHandler : IRequestHandler<ProcessPaymentCommand, bool>
    {
        public async Task<bool> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
        {
            // Ödeme mantığı burada (Bankaya git vb.)
            Console.WriteLine($"{request.OrderId} için {request.TotalPrice} TL ödeme alınıyor...");
            return request.TotalPrice <2000; // test amaçlı 2000'den küçük başarılı, büyük ise başarısız 
        }
    }
}
