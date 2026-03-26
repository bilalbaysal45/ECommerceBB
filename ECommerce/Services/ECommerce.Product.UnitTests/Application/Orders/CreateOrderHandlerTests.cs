using ECommerce.Order.API.Core.Application.Orders.Commands.CreateOrder;
using ECommerce.Order.API.Infrastructure.Persistence;
using ECommerce.Shared.Events;
using MassTransit;
using Moq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Product.UnitTests.Application.Orders
{
    public class CreateOrderHandlerTests
    {
        private readonly Mock<IPublishEndpoint> _publishEndpointMock;

        public CreateOrderHandlerTests()
        {
            _publishEndpointMock = new Mock<IPublishEndpoint>();
        }

        [Fact]
        public async Task Handle_ValidCommand_ShouldCreateOrderAndPublishEvent()
        {
            // 1. Arrange: Gerçek bir DbContext (RAM üzerinde)
            var options = new DbContextOptionsBuilder<OrderDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new OrderDbContext(options);

            // Handler'a gerçek context'i ve Mock'lanmış PublishEndpoint'i veriyoruz
            var handler = new CreateOrderHandler(context, _publishEndpointMock.Object);

            var command = new CreateOrderCommand("user123",new List<Order.API.Models.Dtos.OrderItemCreateDto> { new Order.API.Models.Dtos.OrderItemCreateDto { ProductId = new Guid(),Price = 2, Quantity = 3} });

            // 2. Act
            var result = await handler.Handle(command, CancellationToken.None);

            // 3. Assert
            result.Should().BeTrue();

            // Veritabanına gerçekten kayıt atıldı mı? (Gerçek kontrol)
            context.Orders.Count().Should().Be(1);

            // Mesaj hala dış dünyaya (RabbitMQ) gidiyor mu? (Mock kontrolü devam eder)
            _publishEndpointMock.Verify(x =>
                x.Publish(It.IsAny<OrderCreatedEvent>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}
