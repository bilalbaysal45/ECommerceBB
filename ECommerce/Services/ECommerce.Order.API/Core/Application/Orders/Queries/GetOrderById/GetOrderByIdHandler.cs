using AutoMapper;
using ECommerce.Order.API.Infrastructure.Persistence;
using ECommerce.Order.API.Models.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Order.API.Core.Application.Orders.Queries.GetOrderById
{
    public class GetOrderByIdHandler : IRequestHandler<GetOrderById, OrderDto>
    {
        private readonly OrderDbContext _context;
        private readonly IMapper _mapper;

        public GetOrderByIdHandler(OrderDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<OrderDto> Handle(GetOrderById request, CancellationToken cancellationToken)
        {
            var order = await _context.Orders
                .Include(x => x.OrderItems)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (order == null) throw new KeyNotFoundException();

            return _mapper.Map<OrderDto>(order);
        }
    }
}
