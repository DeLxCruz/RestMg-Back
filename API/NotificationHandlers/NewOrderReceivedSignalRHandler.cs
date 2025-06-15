using Application.Features.Kitchen.Notifications;
using API.Hubs;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace API.NotificationHandlers
{
    public class NewOrderReceivedSignalRHandler(IHubContext<KitchenHub> hubContext)
                : INotificationHandler<NewOrderReceivedNotification>
    {
        private readonly IHubContext<KitchenHub> _hubContext = hubContext;

        public Task Handle(NewOrderReceivedNotification notification, CancellationToken ct)
        {
            var groupName = $"restaurant-{notification.RestaurantId}";
            // Envía un mensaje llamado "NewOrder" a la cocina de ese restaurante
            return _hubContext.Clients.Group(groupName)
                .SendAsync("NewOrder", notification, ct);
        }
    }
}