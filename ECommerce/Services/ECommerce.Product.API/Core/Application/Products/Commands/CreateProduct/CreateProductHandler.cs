using MediatR;
using ECommerce.Product.API.Infrastructure.Persistence;
using ECommerce.Shared.Events;
using MassTransit;
namespace ECommerce.Product.API.Core.Application.Products.Commands.CreateProduct
{
    public class CreateProductHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly ProductDbContext _context;
        private readonly IPublishEndpoint _publishEndpoint;
        public CreateProductHandler(ProductDbContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
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
                IsActive = true,
                CategoryId = request.CategoryId,
                CreatedDate = DateTime.UtcNow
            };

            // Stock.API'nin haberi olsun diye mesaj atıyoruz
            await _publishEndpoint.Publish(new ProductCreatedEvent
            {
                ProductId = product.Id,
                SKU = product.Sku,
                ProductName = product.Name
            }, cancellationToken);

            // Veritabanına ekleme operasyonu
            _context.Products.Add(product);
            await _context.SaveChangesAsync(cancellationToken);

            return product.Id;
        }
    }
}
