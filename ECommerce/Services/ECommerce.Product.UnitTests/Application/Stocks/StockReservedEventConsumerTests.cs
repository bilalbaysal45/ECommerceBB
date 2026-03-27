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
    public class StockReservedEventConsumerTests
    {
        [Fact]
        public async Task Consume_ValidEvent_ShouldUpdateOrderStatusToCompleted()
        {
            // 1. Arrange: InMemory Database Kurulumu
            var options = new DbContextOptionsBuilder<OrderDbContext>()
                .UseInMemoryDatabase(databaseName: "OrderTestDb_" + Guid.NewGuid().ToString())
                .Options;

            using var context = new OrderDbContext(options);

            // Test için başlangıçta 'Suspend' durumunda bir sipariş ekleyelim
            var orderId = Guid.NewGuid();
            var testOrder = new ECommerce.Order.API.Core.Domain.Entities.Order
            {
                Id = orderId,
                UserId = "user-123",
                TotalPrice = 100,
                Status = OrderStatus.Suspend, // Başlangıç durumu
                OrderDate = DateTime.Now,
                OrderItems = new List<OrderItem>()
            };

            context.Orders.Add(testOrder);
            await context.SaveChangesAsync();

            // Consumer ve Mock Context hazırlığı
            var consumer = new StockReservedEventConsumer(context);
            var consumeContextMock = new Mock<ConsumeContext<StockReservedEvent>>();
            consumeContextMock.Setup(x => x.Message).Returns(new StockReservedEvent
            {
                OrderId = orderId
            });

            // 2. Act: Mesajı tüket
            await consumer.Consume(consumeContextMock.Object);

            // 3. Assert: Sonucu doğrula
            var updatedOrder = await context.Orders.FirstOrDefaultAsync(x => x.Id == orderId);

            updatedOrder.Should().NotBeNull();
            updatedOrder!.Status.Should().Be(OrderStatus.Completed); // Durum Completed olmalı!
        }
    }
}
