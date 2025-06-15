using Application.Common.Interfaces;
using Application.Common.Notifications;
using MediatR;

namespace Application.Features.MenuItems.Commands.UpdateMenuItem
{
    public class UpdateMenuItemCommandHandler(IApplicationDbContext db, ICurrentUserService user, IPublisher publisher)
    : IRequestHandler<UpdateMenuItemCommand>
    {
        public async Task Handle(UpdateMenuItemCommand command, CancellationToken ct)
        {
            var restaurantId = user.RestaurantId ?? throw new UnauthorizedAccessException();
            var item = await db.MenuItems.FindAsync(command.Id) ?? throw new KeyNotFoundException("Plato no encontrado.");
            if (item.RestaurantId != restaurantId) throw new UnauthorizedAccessException();

            var oldAvailability = item.IsAvailable;

            item.Name = command.Name;
            item.Description = command.Description;
            item.Price = command.Price;
            item.ImageUrl = command.ImageUrl;
            item.IsAvailable = command.IsAvailable;
            item.CategoryId = command.CategoryId;

            await db.SaveChangesAsync(ct);

            if (oldAvailability != item.IsAvailable)
            {
                await publisher.Publish(new MenuItemAvailabilityNotification(restaurantId, item.Id, item.IsAvailable), ct);
            }
        }
    }
}