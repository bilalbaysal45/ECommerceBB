using ECommerce.Order.API.Core.Application.Orders.Commands.CreateOrder;
using ECommerce.Order.API.Infrastructure.Persistence;
using ECommerce.Shared.Events;
using MassTransit;
using Moq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ECommerce.Shared.Commons;
using MassTransit.Testing;
using ECommerce.Order.API.Core.Domain.Entities;
using ECommerce.Order.API.Models.Dtos;

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
        [Fact]
        public async Task Handle_ValidCommand_ShouldSaveOrderAndEnvelopeMessageInOutbox()
        {
            // 1. Arrange: InMemory Database ve MassTransit Test Harness
            var options = new DbContextOptionsBuilder<OrderDbContext>()
                .UseInMemoryDatabase(databaseName: "OrderOutboxTestDb_" + Guid.NewGuid().ToString())
                .Options;

            using var context = new OrderDbContext(options);

            // MassTransit test araçlarını hazırlıyoruz
            var harness = new InMemoryTestHarness();
            await harness.Start();

            var items = new List<OrderItemCreateDto> { new OrderItemCreateDto {
               ProductId = Guid.NewGuid(), Quantity = 2, Price = 100
            } }; 
            var handler = new CreateOrderHandler(context, harness.Bus);
            var command = new CreateOrderCommand(UserId: "user-789",Items: items);
            

            // 2. Act
            var result = await handler.Handle(command, CancellationToken.None);

            // 3. Assert: Sipariş kaydedildi mi?
            result.Should().BeTrue();
            var savedOrder = await context.Orders.FirstOrDefaultAsync();
            savedOrder.Should().NotBeNull();
            savedOrder!.UserId.Should().Be("user-789");

            // 4. Assert: Mesaj Outbox mantığıyla gönderildi mi?
            // (Harness, PublishEndpoint üzerinden gelen mesajları yakalar)
            var publishedMessage = harness.Published.Select<OrderCreatedEvent>().Any();
            publishedMessage.Should().BeTrue();

            await harness.Stop();
        }
    }
}
