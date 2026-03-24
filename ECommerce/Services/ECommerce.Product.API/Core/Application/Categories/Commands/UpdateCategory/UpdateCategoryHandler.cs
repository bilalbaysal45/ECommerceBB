using ECommerce.Product.API.Infrastructure.Persistence;
using MediatR;

namespace ECommerce.Product.API.Core.Application.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, bool>
    {
        private readonly ProductDbContext _context;
        public UpdateCategoryHandler(ProductDbContext context) => _context = context;

        public async Task<bool> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.Categories.FindAsync(request.Id);
            if (category == null) return false;

            category.Name = request.Name;
            category.Description = request.Description;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
