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

        public Task Handle(MenuItemAvailabilityNotification notification, CancellationToken cancellationToken)
        {
            var groupName = $"restaurant-{notification.RestaurantId}";

            return _hubContext.Clients.Group(groupName)
                .SendAsync("MenuItemAvailabilityUpdated", notification.MenuItemId, notification.IsAvailable, cancellationToken);
        }
    }
}