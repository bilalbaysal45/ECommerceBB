using MediatR;

namespace ECommerce.Product.API.Core.Application.Products.Commands.UpdateProduct
{
    public class UpdateProductCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Sku { get; set; }
        public decimal Price { get; set; }
        public int StockCount { get; set; }
    }
}
