using Application.Common.Interfaces;
using Application.Common.Notifications;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.MenuItems.Commands.UpdateMenuItem
{
    public class UpdateMenuItemCommandHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IPublisher publisher)
        : IRequestHandler<UpdateMenuItemCommand>
    {
        public async Task Handle(UpdateMenuItemCommand command, CancellationToken ct)
        {
            var restaurantId = currentUserService.RestaurantId ?? throw new UnauthorizedAccessException();

            // 1. Validar que el plato a editar existe y pertenece al restaurante.
            var item = await dbContext.MenuItems.FindAsync([command.Id], ct)
                ?? throw new KeyNotFoundException("Plato no encontrado.");

            if (item.RestaurantId != restaurantId)
            {
                throw new UnauthorizedAccessException("No tienes permiso para editar este plato.");
            }

            var categoryExists = await dbContext.Categories
                .AnyAsync(c => c.Id == command.CategoryId && c.RestaurantId == restaurantId, ct);

            if (!categoryExists)
            {
                throw new InvalidOperationException("La categoría seleccionada no es válida o no pertenece a tu restaurante.");
            }

            var availabilityChanged = item.IsAvailable != command.IsAvailable;

            // 3. Aplicar los cambios
            item.Name = command.Name;
            item.Description = command.Description;
            item.Price = command.Price;
            item.ImageUrl = command.ImageUrl;
            item.CategoryId = command.CategoryId;
            item.IsAvailable = command.IsAvailable;

            await dbContext.SaveChangesAsync(ct);

            // 4. Publicar la notificación si es necesario
            if (availabilityChanged)
            {
                Console.WriteLine($"[UpdateMenuItemCommandHandler] ===== Publicando notificación de disponibilidad =====");
                Console.WriteLine($"[UpdateMenuItemCommandHandler] MenuItemId: {item.Id}");
                Console.WriteLine($"[UpdateMenuItemCommandHandler] RestaurantId: {restaurantId}");
                Console.WriteLine($"[UpdateMenuItemCommandHandler] IsAvailable: {item.IsAvailable}");
                Console.WriteLine($"[UpdateMenuItemCommandHandler] Publicando MenuItemAvailabilityNotification...");
                
                await publisher.Publish(new MenuItemAvailabilityNotification(restaurantId, item.Id, item.IsAvailable), ct);
                Console.WriteLine($"[UpdateMenuItemCommandHandler] ✅ Notificación publicada exitosamente");
            }
            else
            {
                Console.WriteLine($"[UpdateMenuItemCommandHandler] Disponibilidad no cambió, no se envía notificación");
            }
        }
    }
}