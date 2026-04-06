using AutoMapper;
using ECommerce.Order.API.Infrastructure.Persistence;
using ECommerce.Order.API.Models.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Order.API.Core.Application.Orders.Queries.GetAllOrders
{
    public class GetAllOrdersQueryHandler :IRequestHandler<GetAllOrdersQuery,List<OrderDto>>
    {
        private readonly OrderDbContext _context;
        private readonly IMapper _mapper;

        public GetAllOrdersQueryHandler(OrderDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<OrderDto>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            var orders = await _context.Orders
                .Where(x => x.UserId == request.buyerId)
                .OrderByDescending(x => x.OrderDate)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<OrderDto>>(orders);
        }
    }
}
