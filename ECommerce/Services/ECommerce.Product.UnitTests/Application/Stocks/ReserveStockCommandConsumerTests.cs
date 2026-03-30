using ECommerce.Shared.Commons;
using ECommerce.Shared.Events;
using ECommerce.Stock.API.Core.Application.Consumers;
using ECommerce.Stock.API.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Moq;
using FluentAssertions;

namespace ECommerce.Product.UnitTests.Application.Stocks
{
    public class ReserveStockCommandConsumerTests
    {
        [Fact]
        public async Task Consume_ValidCommand_ShouldDecrementStockCount()
        {
            // 1. Arrange: Aynı kalıyor (InMemory DB kurulumu)
            var options = new DbContextOptionsBuilder<StockDbContext>()
                .UseInMemoryDatabase(databaseName: "StockTestDb_" + Guid.NewGuid().ToString())
                .Options;

            using var context = new StockDbContext(options);
            var publishEndpointMock = new Mock<IPublishEndpoint>();
            var testStock = new ECommerce.Stock.API.Core.Domain.Entities.Stock
            {
                Id = Guid.NewGuid(),
                ProductId = Guid.NewGuid(),
                StockCount = 100
            };
            context.Stocks.Add(testStock);
            await context.SaveChangesAsync();

            // Consumer'ı YENİ sınıfımızla oluşturuyoruz
            var consumer = new ReserveStockCommandConsumer(context, publishEndpointMock.Object);

            // MassTransit Mock'unu YENİ mesaj tipine (Command) göre güncelliyoruz
            var consumeContextMock = new Mock<ConsumeContext<ReserveStockCommand>>();
            consumeContextMock.Setup(x => x.Message).Returns(new ReserveStockCommand
            {
                OrderId = Guid.NewGuid(),
                OrderItems = new List<OrderItemMessage> // Property isimlerine dikkat!
                {
                    new() { ProductId = testStock.ProductId, Quantity = 20 }
                }
            });

            // 2. Act
            await consumer.Consume(consumeContextMock.Object);

            // 3. Assert
            var updatedStock = await context.Stocks.FirstOrDefaultAsync(x => x.ProductId == testStock.ProductId);
            updatedStock.Should().NotBeNull();
            updatedStock!.StockCount.Should().Be(80);

            consumeContextMock.Verify(x => x.Publish(It.IsAny<StockReservedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
