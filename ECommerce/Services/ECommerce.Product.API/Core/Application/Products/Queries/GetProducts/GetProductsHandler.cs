using AutoMapper;
using ECommerce.Product.API.Core.Application.Dtos;
using ECommerce.Product.API.Core.Domain.Entities;
using ECommerce.Product.API.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ECommerce.Product.API.Core.Application.Products.Queries.GetProducts
{
    public class GetProductsHandler : IRequestHandler<GetProductsQuery, List<ProductDto>>
    {
        private readonly ProductDbContext _context;
        private readonly IMapper _mapper;

        public GetProductsHandler(ProductDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            // Veritabanındaki tüm ürünleri liste olarak çekiyoruz
            var query = _context.Products
                                .Include(p => p.Category)
                                .AsNoTracking()
                                .AsQueryable();

            // Filtreleme Mantığı
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.Where(x => x.Name.Contains(request.SearchTerm) || x.Description.Contains(request.SearchTerm));
            }
            if (request.CategoryId.HasValue)
            {
                query = query.Where(x => x.CategoryId == request.CategoryId.Value);
            }
            if (request.MinPrice.HasValue)
            {
                query = query.Where(x => x.Price >= request.MinPrice.Value);
            }
            if (request.MaxPrice.HasValue)
            {
                query = query.Where(x => x.Price <= request.MaxPrice.Value);
            }

            var products = await query.ToListAsync(cancellationToken);
            return _mapper.Map<List<ProductDto>>(products);
        }
    }
}
