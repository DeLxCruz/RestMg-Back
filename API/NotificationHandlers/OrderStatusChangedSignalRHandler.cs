using API.Hubs;
using Application.Common.Notifications;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace API.NotificationHandlers
{
    public class OrderStatusChangedSignalRHandler(
        IHubContext<KitchenHub> kitchenHubContext,
        IHubContext<NotificationsHub> notificationsHubContext)
              : INotificationHandler<OrderStatusChangedNotification>
    {
        private readonly IHubContext<KitchenHub> _kitchenHubContext = kitchenHubContext;
        private readonly IHubContext<NotificationsHub> _notificationsHubContext = notificationsHubContext;

        public async Task Handle(OrderStatusChangedNotification notification, CancellationToken ct)
        {
            var groupName = $"restaurant-{notification.RestaurantId}";

            // Enviar a KitchenHub
            await _kitchenHubContext.Clients.Group(groupName)
                .SendAsync("OrderStatusUpdated", notification.OrderId, notification.NewStatus, ct);

            // Enviar a NotificationsHub (para meseros/admins)
            await _notificationsHubContext.Clients.Group(groupName)
                .SendAsync("OrderStatusUpdated", notification.OrderId, notification.NewStatus, ct);
        }
    }
}