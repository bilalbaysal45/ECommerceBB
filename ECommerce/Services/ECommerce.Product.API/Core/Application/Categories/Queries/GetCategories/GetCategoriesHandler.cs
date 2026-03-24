using AutoMapper;
using ECommerce.Product.API.Core.Application.Dtos;
using ECommerce.Product.API.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Product.API.Core.Application.Categories.Queries.GetCategories
{
    public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
    {
        private readonly ProductDbContext _context;
        private readonly IMapper _mapper;

        public GetCategoriesHandler(ProductDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _context.Categories.AsNoTracking().ToListAsync(cancellationToken);
            return _mapper.Map<List<CategoryDto>>(categories);
        }
    }
}
