using ECommerce.Order.API.Core.Domain.Enums;
using ECommerce.Order.API.Infrastructure.Persistence;
using ECommerce.Shared.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Order.API.Core.Application.Consumers
{
    public class StockNotEnoughEventConsumer : IConsumer<StockNotEnoughEvent>
    {
        private readonly OrderDbContext _context;
        public StockNotEnoughEventConsumer(OrderDbContext context)
        {
            _context = context;
        }
        public async Task Consume(ConsumeContext<StockNotEnoughEvent> context)
        {
            var message = context.Message;

            // 1. İlgili siparişi bul
            var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == message.OrderId);

            if (order != null)
            {
                // 2. Sipariş durumunu 'Fail' (Hata/Reddedildi) olarak güncelle
                //order.Status = OrderStatus.Fail;
                order.Reject(); //order.Status = OrderStatus.Fail yerine

                await _context.SaveChangesAsync();

                Console.WriteLine($"Sipariş iptal edildi. ID: {message.OrderId}, Sebep: {message.Message}");
            }
        }
    }
}
