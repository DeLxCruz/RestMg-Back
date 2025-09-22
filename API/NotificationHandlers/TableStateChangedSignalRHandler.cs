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

        public async Task Handle(TableStateChangedNotification notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"[TableStateChangedSignalRHandler] ===== Enviando notificación de cambio de estado de mesa =====");
            Console.WriteLine($"[TableStateChangedSignalRHandler] TableId: {notification.TableId}");
            Console.WriteLine($"[TableStateChangedSignalRHandler] RestaurantId: {notification.RestaurantId}");
            Console.WriteLine($"[TableStateChangedSignalRHandler] NewState: {notification.NewState}");
            
            var groupName = $"restaurant-{notification.RestaurantId}";
            Console.WriteLine($"[TableStateChangedSignalRHandler] Enviando a grupo: {groupName}");
            Console.WriteLine($"[TableStateChangedSignalRHandler] Método SignalR: TableStateUpdated");
            
            await _hubContext.Clients.Group(groupName)
                .SendAsync("TableStateUpdated", notification.TableId, notification.NewState, cancellationToken);
                
            Console.WriteLine($"[TableStateChangedSignalRHandler] ✅ Notificación enviada exitosamente");
        }
    }
}