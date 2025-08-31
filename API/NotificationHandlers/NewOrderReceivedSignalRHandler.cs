using Application.Features.Kitchen.Notifications;
using API.Hubs;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Application.Common.Notifications;

namespace API.NotificationHandlers
{
    public class NewOrderReceivedSignalRHandler(IHubContext<NotificationsHub> hubContext)
                : INotificationHandler<NewOrderReceivedNotification>
    {
        private readonly IHubContext<NotificationsHub> _hubContext = hubContext;

        public Task Handle(NewOrderReceivedNotification notification, CancellationToken ct)
        {
            var groupName = $"restaurant-{notification.RestaurantId}";
            // Envía un mensaje llamado "NewOrderForApproval" a los meseros y admins de ese restaurante
            return _hubContext.Clients.Group(groupName)
                .SendAsync("NewOrderForApproval", notification, ct);
        }
    }
}