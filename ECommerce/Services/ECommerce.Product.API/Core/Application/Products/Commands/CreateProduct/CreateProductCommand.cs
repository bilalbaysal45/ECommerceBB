using MediatR;
namespace ECommerce.Product.API.Core.Application.Products.Commands.CreateProduct
{
    public record CreateProductCommand(
        string Name,
        string Description,
        string Sku,
        decimal Price,
        Guid CategoryId
    ) : IRequest<Guid>;
}
