using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace API.Hubs
{
    [Authorize]
    public class KitchenHub : Hub
    {
        // Este método se ejecuta cuando un nuevo cliente se conecta
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"[KitchenHub] ===== Nueva conexión =====");
            Console.WriteLine($"[KitchenHub] ConnectionId: {Context.ConnectionId}");
            Console.WriteLine($"[KitchenHub] UserIdentifier: {Context.UserIdentifier}");
            
            // Se obtiene el ID del restaurante del token JWT del usuario, igual que en CurrentUserService
            var restaurantId = Context.User?.FindFirstValue("restaurantId");
            Console.WriteLine($"[KitchenHub] RestaurantId from token: {restaurantId}");
            
            // Log de todos los claims para debug
            if (Context.User?.Claims != null)
            {
                Console.WriteLine($"[KitchenHub] Claims del usuario:");
                foreach (var claim in Context.User.Claims)
                {
                    Console.WriteLine($"[KitchenHub]   - {claim.Type}: {claim.Value}");
                }
            }

            if (!string.IsNullOrEmpty(restaurantId))
            {
                var groupName = $"restaurant-{restaurantId}";
                Console.WriteLine($"[KitchenHub] Agregando conexión al grupo: {groupName}");
                // Se une la conexión actual a un grupo llamado "restaurant-GUID_DEL_RESTAURANTE"
                await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
                Console.WriteLine($"[KitchenHub] ✅ Conexión agregada exitosamente al grupo {groupName}");
            }
            else
            {
                Console.WriteLine($"[KitchenHub] ❌ No se pudo obtener restaurantId del token");
            }

            await base.OnConnectedAsync();
            Console.WriteLine($"[KitchenHub] ===== Conexión completada =====");
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"[KitchenHub] ===== Desconexión =====");
            Console.WriteLine($"[KitchenHub] ConnectionId: {Context.ConnectionId}");
            Console.WriteLine($"[KitchenHub] UserIdentifier: {Context.UserIdentifier}");
            if (exception != null)
            {
                Console.WriteLine($"[KitchenHub] Exception: {exception.Message}");
            }
            
            var restaurantId = Context.User?.FindFirstValue("restaurantId");
            if (!string.IsNullOrEmpty(restaurantId))
            {
                var groupName = $"restaurant-{restaurantId}";
                Console.WriteLine($"[KitchenHub] Removiendo conexión del grupo: {groupName}");
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            }
            
            await base.OnDisconnectedAsync(exception);
            Console.WriteLine($"[KitchenHub] ===== Desconexión completada =====");
        }
    }
}