using Application.Common.Notifications;
using API.Hubs;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace API.NotificationHandlers
{
    public class TableStateChangedSignalRHandler(IHubContext<KitchenHub> hubContext)
                : INotificationHandler<TableStateChangedNotification>
    {
        private readonly IHubContext<KitchenHub> _hubContext = hubContext;

        public Task Handle(TableStateChangedNotification notification, CancellationToken cancellationToken)
        {
            var groupName = $"restaurant-{notification.RestaurantId}";
            return _hubContext.Clients.Group(groupName)
                .SendAsync("TableStateUpdated", notification.TableId, notification.NewState, cancellationToken);
        }
    }
}