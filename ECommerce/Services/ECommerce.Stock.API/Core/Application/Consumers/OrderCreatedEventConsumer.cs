using ECommerce.Shared.Events;
using ECommerce.Stock.API.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Stock.API.Core.Application.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly StockDbContext _context;

        public OrderCreatedEventConsumer(StockDbContext context)
        {
            _context = context;
        }
        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var message = context.Message;
            // Burada stok düşürme mantığını yazacağız
            Console.WriteLine($"Sipariş alındı! ID: {message.OrderId}, Ürün Sayısı: {message.Items.Count}");

            foreach (var item in message.Items)
            {
                // Veritabanında ilgili ürünü bul
                var stock = await _context.Stocks
                    .FirstOrDefaultAsync(x => x.ProductId == item.ProductId);

                if (stock != null)
                {
                    // Stok miktarını düşür
                    stock.StockCount -= item.Quantity;
                    Console.WriteLine($"Ürün ID: {item.ProductId} için stok güncellendi. Yeni Stok: {stock.StockCount}");
                }
            }

            // Değişiklikleri kaydet
            await _context.SaveChangesAsync();
        }
    }
}
