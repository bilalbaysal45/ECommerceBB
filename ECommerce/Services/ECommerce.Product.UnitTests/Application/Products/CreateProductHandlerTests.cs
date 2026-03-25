using ECommerce.Product.API.Core.Application.Products.Commands.CreateProduct;
using ECommerce.Product.API.Core.Domain.Entities;
using ECommerce.Product.API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class CreateProductHandlerTests
{
    [Fact]
    public async Task Handle_Should_Add_Product_When_Category_Exists()
    {
        // 1. Arrange
        var options = new DbContextOptionsBuilder<ProductDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        using var context = new ProductDbContext(options);

        // Önce bir kategori eklemeliyiz (çünkü Product kategoriye bağlı)
        var category = new Category { Id = Guid.NewGuid(), Name = "Elektronik", Description = "..." };
        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var handler = new CreateProductHandler(context);
        var command = new CreateProductCommand("Laptop", "a", "LPT-001", 1500, 2, category.Id); // Az önce oluşturduğumuz ID
        

        // 2. Act
        var result = await handler.Handle(command, CancellationToken.None);

        // 3. Assert
        Assert.NotEqual(Guid.Empty, result);
        var product = await context.Products.FirstOrDefaultAsync(x => x.Id == result);
        Assert.NotNull(product);
        Assert.Equal("Laptop", product.Name);
    }
}