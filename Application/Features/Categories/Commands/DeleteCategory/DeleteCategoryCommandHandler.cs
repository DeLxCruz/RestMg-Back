using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Categories.Commands.DeleteCategory
{
    public class DeleteCategoryCommandHandler(IApplicationDbContext db, ICurrentUserService user)
    : IRequestHandler<DeleteCategoryCommand>
    {
        public async Task Handle(DeleteCategoryCommand command, CancellationToken ct)
        {
            var restaurantId = user.RestaurantId ?? throw new UnauthorizedAccessException();
            var category = await db.Categories.FindAsync(command.Id) ?? throw new KeyNotFoundException("Categoría no encontrada.");
            if (category.RestaurantId != restaurantId) throw new UnauthorizedAccessException();

            category.IsActive = false;
            await db.SaveChangesAsync(ct);
        }
    }
}