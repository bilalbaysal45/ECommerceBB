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

            // 1. Tüm ilgili stok kayıtlarını çek
            var productIds = message.Items.Select(x => x.ProductId).ToList();
            var stocks = await _context.Stocks
                .Where(s => productIds.Contains(s.ProductId))
                .ToListAsync();
            // 2. Kontrol et
            bool isStockAvailable = message.Items.All(item =>
            {
                var stock = stocks.FirstOrDefault(s => s.ProductId == item.ProductId);
                return stock != null && stock.StockCount >= item.Quantity;
            });

            if (isStockAvailable)
            {
                // 3. Stokları düşür (Elimizdeki 'stocks' listesi üzerinden)
                foreach (var item in message.Items)
                {
                    var stock = stocks.First(s => s.ProductId == item.ProductId);
                    stock.StockCount -= item.Quantity;
                }
                // Değişiklikleri kaydet
                await _context.SaveChangesAsync();
                Console.WriteLine($"Stok güncellendi. Sipariş ID: {message.OrderId}");
                await context.Publish(new StockReservedEvent
                {
                    OrderId = message.OrderId
                });
            }
            else
            {
                // 4. Stok Yetersizse Yayınla
                await context.Publish(new StockNotEnoughEvent
                {
                    OrderId = message.OrderId,
                    Message = "Stok yetersizliği nedeniyle sipariş reddedildi."
                });
                Console.WriteLine($"Stok yetersiz! Sipariş ID: {message.OrderId}");
            }
        }
    }
}
