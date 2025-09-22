using Application.Common.Notifications;
using API.Hubs;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace API.NotificationHandlers
{
    public class MenuItemAvailabilitySignalRHandler(IHubContext<KitchenHub> hubContext)
                : INotificationHandler<MenuItemAvailabilityNotification>
    {
        private readonly IHubContext<KitchenHub> _hubContext = hubContext;

        public async Task Handle(MenuItemAvailabilityNotification notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"[MenuItemAvailabilitySignalRHandler] ===== Enviando notificación de disponibilidad de item =====");
            Console.WriteLine($"[MenuItemAvailabilitySignalRHandler] MenuItemId: {notification.MenuItemId}");
            Console.WriteLine($"[MenuItemAvailabilitySignalRHandler] RestaurantId: {notification.RestaurantId}");
            Console.WriteLine($"[MenuItemAvailabilitySignalRHandler] IsAvailable: {notification.IsAvailable}");
            
            var groupName = $"restaurant-{notification.RestaurantId}";
            Console.WriteLine($"[MenuItemAvailabilitySignalRHandler] Enviando a grupo: {groupName}");
            Console.WriteLine($"[MenuItemAvailabilitySignalRHandler] Método SignalR: MenuItemAvailabilityUpdated");

            await _hubContext.Clients.Group(groupName)
                .SendAsync("MenuItemAvailabilityUpdated", notification.MenuItemId, notification.IsAvailable, cancellationToken);
                
            Console.WriteLine($"[MenuItemAvailabilitySignalRHandler] ✅ Notificación enviada exitosamente");
        }
    }
}