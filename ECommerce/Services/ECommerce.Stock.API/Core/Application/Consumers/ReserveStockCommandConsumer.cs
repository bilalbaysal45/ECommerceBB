using ECommerce.Shared.Events;
using ECommerce.Stock.API.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Stock.API.Core.Application.Consumers
{
    public class ReserveStockCommandConsumer : IConsumer<ReserveStockCommand>
    {
        private readonly StockDbContext _context;
        private readonly IPublishEndpoint _publishEndpoint;

        public ReserveStockCommandConsumer(StockDbContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<ReserveStockCommand> context)
        {
            var message = context.Message;
            Console.WriteLine($"Saga'dan stok rezervasyon komutu alındı. Sipariş ID: {message.OrderId}");

            var productIds = message.OrderItems.Select(x => x.ProductId).ToList();

            // 2. Stok kayıtlarını getir
            var stocks = await _context.Stocks
                .Where(s => productIds.Contains(s.ProductId))
                .ToListAsync();

            // 3. Stok yeterlilik kontrolü
            bool isStockAvailable = message.OrderItems.All(item =>
            {
                var stock = stocks.FirstOrDefault(s => s.ProductId == item.ProductId);
                return stock != null && stock.StockCount >= item.Quantity;
            });

            if (isStockAvailable)
            {
                // 4. Stokları düşür
                foreach (var item in message.OrderItems)
                {
                    var stock = stocks.First(s => s.ProductId == item.ProductId);
                    stock.StockCount -= item.Quantity;
                }

                await _context.SaveChangesAsync();

                Console.WriteLine($"Stok başarıyla rezerve edildi. Sipariş ID: {message.OrderId}");

                // Saga'ya "İşlem Tamam" bilgisini gönder
                await context.Publish(new StockReservedEvent
                {
                    OrderId = message.OrderId
                });
            }
            else
            {
                // 5. Stok yetersizse Saga'ya "Hata" bilgisini gönder
                Console.WriteLine($"Stok yetersiz! Sipariş ID: {message.OrderId}");

                await context.Publish(new StockNotEnoughEvent
                {
                    OrderId = message.OrderId,
                    Message = "Stokta yeterli ürün bulunamadı."
                });
            }
        }
    }
}
