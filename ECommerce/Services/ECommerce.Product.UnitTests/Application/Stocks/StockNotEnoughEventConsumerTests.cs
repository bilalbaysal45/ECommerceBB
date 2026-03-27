using ECommerce.Order.API.Core.Application.Consumers;
using ECommerce.Order.API.Core.Domain.Entities;
using ECommerce.Order.API.Core.Domain.Enums;
using ECommerce.Order.API.Infrastructure.Persistence;
using ECommerce.Shared.Events;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Moq;
using FluentAssertions;

namespace ECommerce.Product.UnitTests.Application.Stocks
{
    public class StockNotEnoughEventConsumerTests
    {
        [Fact]
        public async Task Consume_StockNotEnoughEvent_ShouldUpdateOrderStatusToFail()
        {
            // 1. Arrange: InMemory Database Kurulumu
            var options = new DbContextOptionsBuilder<OrderDbContext>()
                .UseInMemoryDatabase(databaseName: "OrderTestDb_Fail_" + Guid.NewGuid().ToString())
                .Options;

            using var context = new OrderDbContext(options);

            var items = new List<OrderItem>
            {
                new OrderItem { ProductId = Guid.NewGuid(), Quantity = 5, UnitPrice = 50 }
            };

            var testOrder = new ECommerce.Order.API.Core.Domain.Entities.Order("user-456", items);

            context.Orders.Add(testOrder);
            await context.SaveChangesAsync();

            var consumer = new StockNotEnoughEventConsumer(context);
            var consumeContextMock = new Mock<ConsumeContext<StockNotEnoughEvent>>();

            consumeContextMock.Setup(x => x.Message).Returns(new StockNotEnoughEvent
            {
                OrderId = testOrder.Id,
                Message = "Yetersiz stok nedeniyle iptal."
            });

            // 2. Act: Mesajı tüket (Consume)
            await consumer.Consume(consumeContextMock.Object);

            // 3. Assert: Sonucu doğrula
            var updatedOrder = await context.Orders.FirstOrDefaultAsync(x => x.Id == testOrder.Id);

            updatedOrder.Should().NotBeNull();
            updatedOrder!.Status.Should().Be(OrderStatus.Fail);
        }
    }
}
