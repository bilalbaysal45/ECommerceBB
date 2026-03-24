using ECommerce.Product.API.Core.Application.Dtos;
using MediatR;

namespace ECommerce.Product.API.Core.Application.Categories.Queries.GetCategories
{
    public class GetCategoriesQuery : IRequest<List<CategoryDto>>
    {
    }
}
