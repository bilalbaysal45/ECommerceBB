using MediatR;
using ECommerce.Product.API.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using ECommerce.Product.API.Infrastructure.Persistence;
namespace ECommerce.Product.API.Core.Application.Products.Commands.CreateProduct
{
    public class CreateProductHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly ProductDbContext _context;
        public CreateProductHandler(ProductDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            // 1. Domain Entity'mizi oluşturuyoruz
            // new Domain.Entities.Product bu şekilde yazmamın sebebi ECommerce.Product proje adımla Çakışması isim çakışması
            var product = new Domain.Entities.Product
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Sku = request.Sku,
                Price = request.Price,
                StockCount = request.StockCount,
                IsActive = true,
                CategoryId = request.CategoryId,
                CreatedDate = DateTime.UtcNow
            };


            // Veritabanına ekleme operasyonu
            _context.Products.Add(product);
            await _context.SaveChangesAsync(cancellationToken);

            return product.Id;
        }
    }
}
