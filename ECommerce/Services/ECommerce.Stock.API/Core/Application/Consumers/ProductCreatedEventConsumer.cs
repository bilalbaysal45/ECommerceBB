using ECommerce.Shared.Events;
using ECommerce.Stock.API.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Stock.API.Core.Application.Consumers
{
    public class ProductCreatedEventConsumer : IConsumer<ProductCreatedEvent>
    {
        private readonly StockDbContext _context;

        public ProductCreatedEventConsumer(StockDbContext context)
        {
            _context = context;
        }
        public async Task Consume(ConsumeContext<ProductCreatedEvent> context)
        {
            var message = context.Message;

            // InboxState (Idempotency) kontrolü: Bu ürün zaten eklenmiş mi?
            var anyStock = await _context.Stocks.AnyAsync(x => x.ProductId == message.ProductId);

            if (!anyStock)
            {
                var newStock = new Domain.Entities.Stock
                {
                    ProductId = message.ProductId,
                    StockCount = 0 // Yeni ürün her zaman 0 stokla başlar, sonra güncellenir.
                };

                _context.Stocks.Add(newStock);
                await _context.SaveChangesAsync();
            }
        }
    }
}
