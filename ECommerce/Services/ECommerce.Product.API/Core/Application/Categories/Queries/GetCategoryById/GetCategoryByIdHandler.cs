using AutoMapper;
using ECommerce.Product.API.Core.Application.Dtos;
using ECommerce.Product.API.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Product.API.Core.Application.Categories.Queries.GetCategoryById
{
    public class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
    {
        private readonly ProductDbContext _context;
        private readonly IMapper _mapper;
        public GetCategoryByIdHandler(ProductDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var category = await _context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
            return _mapper.Map<CategoryDto>(category);
        }
    }
}
