using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace API.Hubs
{
    [Authorize]
    public class NotificationsHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"[NotificationsHub] ===== Nueva conexión =====");
            Console.WriteLine($"[NotificationsHub] ConnectionId: {Context.ConnectionId}");
            Console.WriteLine($"[NotificationsHub] UserIdentifier: {Context.UserIdentifier}");
            
            var restaurantId = Context.User?.Claims.FirstOrDefault(c => c.Type == "restaurantId")?.Value;
            Console.WriteLine($"[NotificationsHub] RestaurantId from token: {restaurantId}");
            
            // Log de todos los claims para debug
            if (Context.User?.Claims != null)
            {
                Console.WriteLine($"[NotificationsHub] Claims del usuario:");
                foreach (var claim in Context.User.Claims)
                {
                    Console.WriteLine($"[NotificationsHub]   - {claim.Type}: {claim.Value}");
                }
            }
            
            if (!string.IsNullOrEmpty(restaurantId))
            {
                var groupName = $"restaurant-{restaurantId}";
                Console.WriteLine($"[NotificationsHub] Agregando conexión al grupo: {groupName}");
                // Agrupar conexiones por restaurante
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                Console.WriteLine($"[NotificationsHub] ✅ Conexión agregada exitosamente al grupo {groupName}");
            }
            else
            {
                Console.WriteLine($"[NotificationsHub] ❌ No se pudo obtener restaurantId del token");
            }
            
            await base.OnConnectedAsync();
            Console.WriteLine($"[NotificationsHub] ===== Conexión completada =====");
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"[NotificationsHub] ===== Desconexión =====");
            Console.WriteLine($"[NotificationsHub] ConnectionId: {Context.ConnectionId}");
            Console.WriteLine($"[NotificationsHub] UserIdentifier: {Context.UserIdentifier}");
            if (exception != null)
            {
                Console.WriteLine($"[NotificationsHub] Exception: {exception.Message}");
            }
            
            var restaurantId = Context.User?.Claims.FirstOrDefault(c => c.Type == "restaurantId")?.Value;
            if (!string.IsNullOrEmpty(restaurantId))
            {
                var groupName = $"restaurant-{restaurantId}";
                Console.WriteLine($"[NotificationsHub] Removiendo conexión del grupo: {groupName}");
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            }
            
            await base.OnDisconnectedAsync(exception);
            Console.WriteLine($"[NotificationsHub] ===== Desconexión completada =====");
        }
    }
}
