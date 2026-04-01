using ECommerce.Stock.API.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Stock.API.Core.Application.CompensateStocks
{
    public class CompensateStockHandler : IRequestHandler<CompensateStockCommand,bool>
    {
        private readonly StockDbContext _context;
        public CompensateStockHandler(StockDbContext context) => _context = context;

        public async Task<bool> Handle(CompensateStockCommand request, CancellationToken cancellationToken)
        {
            foreach (var item in request.OrderItems)
            {
                var stock = await _context.Stocks.FirstOrDefaultAsync(x => x.ProductId == item.ProductId);
                if (stock != null)
                {
                    stock.StockCount += item.Quantity; // Stokları geri iade ediyoruz!
                }
            }
            await _context.SaveChangesAsync();
            Console.WriteLine($"[Stock API] {request.OrderId} nolu siparişin stokları iade edildi.");
            return true;
        }
    }
}
