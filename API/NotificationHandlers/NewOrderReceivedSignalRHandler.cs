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

        public async Task Handle(NewOrderReceivedNotification notification, CancellationToken ct)
        {
            Console.WriteLine($"[NewOrderReceivedSignalRHandler] ===== Enviando notificación de nueva orden =====");
            Console.WriteLine($"[NewOrderReceivedSignalRHandler] OrderId: {notification.OrderId}");
            Console.WriteLine($"[NewOrderReceivedSignalRHandler] RestaurantId: {notification.RestaurantId}");
            Console.WriteLine($"[NewOrderReceivedSignalRHandler] TableCode: {notification.TableCode}");
            Console.WriteLine($"[NewOrderReceivedSignalRHandler] OrderCode: {notification.OrderCode}");
            Console.WriteLine($"[NewOrderReceivedSignalRHandler] Total: ${notification.Total}");
            
            var groupName = $"restaurant-{notification.RestaurantId}";
            Console.WriteLine($"[NewOrderReceivedSignalRHandler] Enviando a grupo: {groupName}");
            Console.WriteLine($"[NewOrderReceivedSignalRHandler] Método SignalR: NewOrderReceived");
            
            // Envía un mensaje llamado "NewOrderReceived" a los meseros y admins de ese restaurante
            await _hubContext.Clients.Group(groupName)
                .SendAsync("NewOrderReceived", notification, ct);
                
            Console.WriteLine($"[NewOrderReceivedSignalRHandler] ✅ Notificación enviada exitosamente");
        }
    }
}