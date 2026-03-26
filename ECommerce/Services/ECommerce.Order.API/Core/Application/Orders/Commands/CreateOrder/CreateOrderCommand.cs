using ECommerce.Order.API.Models.Dtos;
using MediatR;

namespace ECommerce.Order.API.Core.Application.Orders.Commands.CreateOrder
{
    public record CreateOrderCommand(string UserId, List<OrderItemCreateDto> Items) : IRequest<bool>;
}
