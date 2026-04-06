using ECommerce.Order.API.Models.Dtos;
using MediatR;

namespace ECommerce.Order.API.Core.Application.Orders.Queries.GetAllOrders
{
    public record GetAllOrdersQuery(string buyerId):IRequest<List<OrderDto>>;
}
