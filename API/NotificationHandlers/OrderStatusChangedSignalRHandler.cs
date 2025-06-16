using API.Hubs;
using Application.Common.Notifications;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace API.NotificationHandlers
{
    public class OrderStatusChangedSignalRHandler(IHubContext<KitchenHub> hubContext)
              : INotificationHandler<OrderStatusChangedNotification>
    {
        private readonly IHubContext<KitchenHub> _hubContext = hubContext;

        public Task Handle(OrderStatusChangedNotification notification, CancellationToken ct)
        {
            var groupName = $"restaurant-{notification.RestaurantId}";
            return _hubContext.Clients.Group(groupName)
                .SendAsync("OrderStatusUpdated", notification.OrderId, notification.NewStatus, ct);
        }
    }
}