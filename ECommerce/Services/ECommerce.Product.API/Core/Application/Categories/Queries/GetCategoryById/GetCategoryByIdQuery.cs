using ECommerce.Product.API.Core.Application.Dtos;
using MediatR;

namespace ECommerce.Product.API.Core.Application.Categories.Queries.GetCategoryById
{
    public class GetCategoryByIdQuery : IRequest<CategoryDto>
    {
        public Guid Id { get; set; }
        public GetCategoryByIdQuery(Guid id) => Id = id;
    }
}
