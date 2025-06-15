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
            // Se obtiene el ID del restaurante del token JWT del usuario, igual que en CurrentUserService
            var restaurantId = Context.User?.FindFirstValue("restaurantId");

            if (!string.IsNullOrEmpty(restaurantId))
            {
                // Se une la conexión actual a un grupo llamado "restaurant-GUID_DEL_RESTAURANTE"
                await Groups.AddToGroupAsync(Context.ConnectionId, $"restaurant-{restaurantId}");
            }

            await base.OnConnectedAsync();
        }
    }
}