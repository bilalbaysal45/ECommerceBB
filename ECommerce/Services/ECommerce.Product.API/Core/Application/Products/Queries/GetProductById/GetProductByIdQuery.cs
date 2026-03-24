using ECommerce.Product.API.Core.Application.Dtos;
using MediatR;

namespace ECommerce.Product.API.Core.Application.Products.Queries.GetProductById
{
    public class GetProductByIdQuery : IRequest<ProductDto>
    {
        public Guid Id { get; set; }
        public GetProductByIdQuery(Guid id) 
        {
            Id = id;
        }
    }
}
