using ECommerce.Order.API.Models.Dtos;
using MediatR;

namespace ECommerce.Order.API.Core.Application.Orders.Queries.GetOrderById
{
    public record GetOrderById(Guid Id) : IRequest<OrderDto>
    {
    }
}
