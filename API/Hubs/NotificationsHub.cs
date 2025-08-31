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
            var restaurantId = Context.User?.Claims.FirstOrDefault(c => c.Type == "restaurantId")?.Value;
            if (!string.IsNullOrEmpty(restaurantId))
            {
                // Agrupar conexiones por restaurante
                await Groups.AddToGroupAsync(Context.ConnectionId, $"Restaurant-{restaurantId}");
            }
            await base.OnConnectedAsync();
        }
    }
}
