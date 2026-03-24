using ECommerce.Product.API.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Product.API.Core.Application.Products.Commands.UpdateProduct
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, bool>
    {
        private readonly ProductDbContext _context;
        public UpdateProductHandler(ProductDbContext context) 
        {
            _context = context;
        }
        public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (product == null)
                return false;

            // Alanları güncelleme
            product.Name = request.Name;
            product.Description = request.Description;
            product.Sku = request.Sku;
            product.Price = request.Price;
            product.StockCount = request.StockCount;
            product.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
