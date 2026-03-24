using ECommerce.Product.API.Core.Application.Dtos;
using MediatR;

namespace ECommerce.Product.API.Core.Application.Products.Queries.GetProducts
{
    // List<Product> dönecek bir sorgu tanımlıyoruz
    public class GetProductsQuery : IRequest<List<ProductDto>>
    {
    }
}
