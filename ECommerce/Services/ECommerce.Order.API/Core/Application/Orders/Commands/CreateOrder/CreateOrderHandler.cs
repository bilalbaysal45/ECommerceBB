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

            // Siparişi yeni constructor ile yaratıyoruz
            // Id, CreatedDate, Status ve TotalPrice hesaplamasını artık Order sınıfı kendi içinde yapacak!
            var order = new DomainOrder(request.UserId, items);

            //var order = new DomainOrder
            //{
            //    Id = Guid.NewGuid(),
            //    UserId = request.UserId,
            //    OrderDate = DateTime.UtcNow,
            //    TotalPrice = request.Items.Sum(x => x.Price * x.Quantity),
            //    OrderItems = request.Items.Select(x => new OrderItem
            //    {
            //        Id = Guid.NewGuid(),
            //        ProductId = x.ProductId,
            //        Quantity = x.Quantity,
            //        UnitPrice = x.Price
            //    }).ToList()
            //};

            // 2. Veritabanına Kaydet
            _context.Orders.Add(order);
            var result = await _context.SaveChangesAsync(cancellationToken);

            if (result > 0)
            {
                // 3. RABBITMQ: Sipariş oluştu mesajını fırlat!
                await _publishEndpoint.Publish(new OrderCreatedEvent
                {
                    OrderId = order.Id,
                    UserId = order.UserId,
                    Items = order.OrderItems.Select(x => new OrderItemMessage
                    {
                        ProductId = x.ProductId,
                        Quantity = x.Quantity
                    }).ToList()
                }, cancellationToken);

                return true;
            }

            return false;
        }
    }
}
