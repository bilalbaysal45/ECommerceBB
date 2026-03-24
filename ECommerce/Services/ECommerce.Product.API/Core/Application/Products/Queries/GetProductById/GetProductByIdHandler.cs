using AutoMapper;
using ECommerce.Product.API.Core.Application.Dtos;
using ECommerce.Product.API.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Product.API.Core.Application.Products.Queries.GetProductById
{
    public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
    {
        private readonly ProductDbContext _context;
        private readonly IMapper _mapper;
        public GetProductByIdHandler(ProductDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (product == null)
            {
                // Şimdilik null dönüyoruz, ileride "NotFoundException" fırlatmayı öğreneceğiz
                return null;
            }

            return _mapper.Map<ProductDto>(product);
        }
    }
}
