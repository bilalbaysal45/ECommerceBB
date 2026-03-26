using ECommerce.Order.API.Core.Domain.Enums;
using ECommerce.Order.API.Infrastructure.Persistence;
using ECommerce.Shared.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Order.API.Core.Application.Consumers
{
    public class StockReservedEventConsumer : IConsumer<StockReservedEvent>
    {
        private readonly OrderDbContext _context;

    public StockReservedEventConsumer(OrderDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<StockReservedEvent> context)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == context.Message.OrderId);

        if (order != null)
        {
            order.Status = OrderStatus.Completed; // Başarı!
            await _context.SaveChangesAsync();
            Console.WriteLine($"Sipariş başarıyla tamamlandı: {order.Id}");
        }
    }
}
}
