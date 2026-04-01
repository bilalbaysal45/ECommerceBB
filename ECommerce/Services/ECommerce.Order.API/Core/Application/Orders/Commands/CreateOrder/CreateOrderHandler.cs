using ECommerce.Order.API.Core.Domain.Entities;
using ECommerce.Order.API.Infrastructure.Persistence;
using ECommerce.Shared.Commons;
using ECommerce.Shared.Events;
using MassTransit;
using MediatR;
using DomainOrder = ECommerce.Order.API.Core.Domain.Entities.Order;

namespace ECommerce.Order.API.Core.Application.Orders.Commands.CreateOrder
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, bool>
    {
        private readonly OrderDbContext _context;
        private readonly IPublishEndpoint _publishEndpoint;

        public CreateOrderHandler(OrderDbContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
        }
        public async Task<bool> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            // 1. Önce OrderItem listeni hazırlıyoruz
            var items = request.Items.Select(x => new OrderItem
            {
                ProductId = x.ProductId,
                Quantity = x.Quantity,
                UnitPrice = x.Price
            }).ToList();

            var order = new DomainOrder(request.UserId, items);

            // 2. Veritabanına Kaydet
            _context.Orders.Add(order);


            // 3. MESAJI BURADA (DB'YE YAZMADAN HEMEN ÖNCE) PUBLISH ET
            // Outbox devrede olduğu için bu mesaj RabbitMQ'ya gitmeyecek, 
            // Context içindeki 'OutboxMessages' tablosuna yazılmak üzere sıraya alınacak.
            var totalPrice = GetTotalPrice(order.OrderItems);
            await _publishEndpoint.Publish(new OrderCreatedEvent
                {
                    OrderId = order.Id,
                    UserId = order.UserId,
                    TotalPrice = totalPrice,
                    Items = order.OrderItems.Select(x => new OrderItemMessage
                    {
                        ProductId = x.ProductId,
                        Quantity = x.Quantity,
                        Price = x.UnitPrice
                    }).ToList()
                }, cancellationToken);
            var result = await _context.SaveChangesAsync(cancellationToken);

            return result > 0;
        }
        private decimal GetTotalPrice(List<OrderItem> items)
        {
            decimal totalPrice = 0;
            foreach (var item in items)
            {
                totalPrice += item.Quantity * item.UnitPrice;
            }
            return totalPrice;
        }
    }
}
