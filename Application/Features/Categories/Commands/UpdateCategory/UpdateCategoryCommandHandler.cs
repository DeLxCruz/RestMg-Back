using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Categories.Commands.UpdateCategory
{
    public class UpdateCategoryCommandHandler(IApplicationDbContext db, ICurrentUserService user)
    : IRequestHandler<UpdateCategoryCommand>
    {
        public async Task Handle(UpdateCategoryCommand command, CancellationToken ct)
        {
            var restaurantId = user.RestaurantId ?? throw new UnauthorizedAccessException();
            var category = await db.Categories.FindAsync(command.Id) ?? throw new KeyNotFoundException("Categoría no encontrada.");
            if (category.RestaurantId != restaurantId) throw new UnauthorizedAccessException();

            category.Name = command.Name;
            category.DisplayOrder = command.DisplayOrder;
            await db.SaveChangesAsync(ct);
        }
    }
}