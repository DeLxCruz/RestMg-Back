using Application.Common.Interfaces;
using Domain.Models;
using MediatR;

namespace Application.Features.MenuItems.Commands.CreateMenuItem
{
    public class CreateMenuItemCommandHandler(IApplicationDbContext db, ICurrentUserService user)
    : IRequestHandler<CreateMenuItemCommand, Guid>
    {
        public async Task<Guid> Handle(CreateMenuItemCommand command, CancellationToken ct)
        {
            var restaurantId = user.RestaurantId ?? throw new UnauthorizedAccessException();
            // Validar que la categoría pertenece al restaurante
            var category = await db.Categories.FindAsync(command.CategoryId) ?? throw new KeyNotFoundException("Categoría no encontrada.");
            if (category.RestaurantId != restaurantId) throw new UnauthorizedAccessException();

            var item = new MenuItem
            {
                Name = command.Name,
                Description = command.Description,
                Price = command.Price,
                ImageUrl = command.ImageUrl,
                CategoryId = command.CategoryId,
                RestaurantId = restaurantId
            };
            await db.MenuItems.AddAsync(item, ct);
            await db.SaveChangesAsync(ct);
            return item.Id;
        }
    }
}