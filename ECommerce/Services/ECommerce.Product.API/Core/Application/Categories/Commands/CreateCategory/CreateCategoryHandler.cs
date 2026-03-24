using ECommerce.Product.API.Core.Domain.Entities;
using ECommerce.Product.API.Infrastructure.Persistence;
using MediatR;

namespace ECommerce.Product.API.Core.Application.Categories.Commands.CreateCategory
{
    public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, Guid>
    {
        private readonly ProductDbContext _context;
        public CreateCategoryHandler(ProductDbContext context)
        {
            _context = context;
        }
        public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync(cancellationToken);

            return category.Id;
        }
    }
}
