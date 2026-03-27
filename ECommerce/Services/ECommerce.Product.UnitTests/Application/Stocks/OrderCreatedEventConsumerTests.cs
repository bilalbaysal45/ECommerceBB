using ECommerce.Shared.Events;
using MassTransit;
using Moq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ECommerce.Stock.API.Infrastructure.Persistence;
using ECommerce.Stock.API.Core.Application.Consumers;
using ECommerce.Shared.Commons;

namespace ECommerce.Product.UnitTests.Application.Stocks
{
    public class OrderCreatedEventConsumerTests
    {
        [Fact]
        public async Task Consume_ValidEvent_ShouldDecrementStockCount()
        {
            // 1. Arrange: RAM üzerinde izole bir veritabanı kurulumu
            var options = new DbContextOptionsBuilder<StockDbContext>()
                .UseInMemoryDatabase(databaseName: "StockTestDb_" + Guid.NewGuid().ToString())
                .Options;

            using var context = new StockDbContext(options);

            // Test için başlangıç verisi ekleyelim ( Stok: 100)
            var testStock = new ECommerce.Stock.API.Core.Domain.Entities.Stock
            {
                Id = Guid.NewGuid(),
                ProductId = Guid.NewGuid(),
                StockCount = 100
            };
            context.Stocks.Add(testStock);
            await context.SaveChangesAsync();

            // Consumer'ı gerçek context ile oluşturuyoruz
            var consumer = new OrderCreatedEventConsumer(context);

            // MassTransit'in ConsumeContext'ini Mock'lamamız gerekiyor
            var consumeContextMock = new Mock<ConsumeContext<OrderCreatedEvent>>();
            consumeContextMock.Setup(x => x.Message).Returns(new OrderCreatedEvent
            {
                OrderId = Guid.NewGuid(),
                Items = new List<OrderItemMessage>
            {
                new() { ProductId = testStock.ProductId, Quantity = 20 } // 20 tane düşmesini bekliyoruz
            }
            });

            // 2. Act: Mesajı tüket (Consume)
            await consumer.Consume(consumeContextMock.Object);

            // 3. Assert: Sonucu doğrula
            var updatedStock = await context.Stocks.FirstOrDefaultAsync(x => x.ProductId == testStock.ProductId);
            updatedStock.Should().NotBeNull();
            updatedStock!.StockCount.Should().Be(80); // 100 - 20 = 80 olmalı
        }
    }
}
