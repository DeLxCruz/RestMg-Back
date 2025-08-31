using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface INotificationsHub
    {
        Task SendNewOrderNotification(string restaurantId, object order);
    }
}
