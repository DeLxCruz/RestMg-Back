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
            Console.WriteLine($"[OrderStatusChangedSignalRHandler] ===== Enviando notificación de cambio de estado =====");
            Console.WriteLine($"[OrderStatusChangedSignalRHandler] OrderId: {notification.OrderId}");
            Console.WriteLine($"[OrderStatusChangedSignalRHandler] RestaurantId: {notification.RestaurantId}");
            Console.WriteLine($"[OrderStatusChangedSignalRHandler] NewStatus: {notification.NewStatus}");
            
            var groupName = $"restaurant-{notification.RestaurantId}";
            Console.WriteLine($"[OrderStatusChangedSignalRHandler] Enviando a grupo: {groupName}");
            Console.WriteLine($"[OrderStatusChangedSignalRHandler] Método SignalR: OrderStatusUpdated");

            // Enviar a KitchenHub
            Console.WriteLine($"[OrderStatusChangedSignalRHandler] Enviando a KitchenHub...");
            await _kitchenHubContext.Clients.Group(groupName)
                .SendAsync("OrderStatusUpdated", notification.OrderId, notification.NewStatus, ct);
            Console.WriteLine($"[OrderStatusChangedSignalRHandler] ✅ Enviado a KitchenHub");

            // Enviar a NotificationsHub (para meseros/admins)
            Console.WriteLine($"[OrderStatusChangedSignalRHandler] Enviando a NotificationsHub...");
            await _notificationsHubContext.Clients.Group(groupName)
                .SendAsync("OrderStatusUpdated", notification.OrderId, notification.NewStatus, ct);
            Console.WriteLine($"[OrderStatusChangedSignalRHandler] ✅ Enviado a NotificationsHub");
            
            Console.WriteLine($"[OrderStatusChangedSignalRHandler] ===== Notificaciones enviadas exitosamente =====");
        }
    }
}