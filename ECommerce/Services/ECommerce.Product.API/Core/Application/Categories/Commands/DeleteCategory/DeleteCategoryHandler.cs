using ECommerce.Product.API.Infrastructure.Persistence;
using MediatR;

namespace ECommerce.Product.API.Core.Application.Categories.Commands.DeleteCategory
{
    public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, bool>
    {
        private readonly ProductDbContext _context;
        public DeleteCategoryHandler(ProductDbContext context) => _context = context;
        public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _context.Categories.FindAsync(request.Id);
            if (category == null) return false;

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
