using ECommerce.Product.API.Core.Application.Categories.Commands.CreateCategory;
using ECommerce.Product.API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ECommerce.Product.UnitTests.Application.Categories
{
    public class CreateCategoryHandlerTests
    {
        [Fact]
        public async Task Handle_Should_Add_Category_To_Database()
        {
            // 1. Arrange
            var options = new DbContextOptionsBuilder<ProductDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Her test için sıfır DB
                .Options;

            using (var context = new ProductDbContext(options))
            {
                var handler = new CreateCategoryHandler(context);
                var command = new CreateCategoryCommand { Name = "Test Kategori", Description = "Test" };

                // 2. Act
                var result = await handler.Handle(command, CancellationToken.None);

                // 3. Assert
                Assert.NotEqual(Guid.Empty, result);
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == result);
                Assert.NotNull(category);
                Assert.Equal("Test Kategori", category.Name);
            }
        }
    }
}