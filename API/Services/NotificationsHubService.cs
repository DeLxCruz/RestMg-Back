using API.Hubs;
using Application.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace API.Services
{
    public class NotificationsHubService(IHubContext<NotificationsHub> hubContext) : INotificationsHub
    {
        public async Task SendNewOrderNotification(string restaurantId, object order)
        {
            await hubContext.Clients.Group($"Restaurant-{restaurantId}").SendAsync("NewOrderReceived", order);
        }
    }
}
