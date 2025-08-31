using Application.Common.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Common.Notifications
{
    public class NewOrderReceivedSignalRHandler(INotificationsHub notificationsHub)
        : INotificationHandler<NewOrderReceivedNotification>
    {
        public Task Handle(NewOrderReceivedNotification notification, CancellationToken cancellationToken)
        {
            var orderPayload = new
            {
                notification.OrderId,
                notification.OrderCode,
                notification.Status,
                notification.Total,
                notification.CreatedAt,
                notification.TableCode
            };

            return notificationsHub.SendNewOrderNotification(notification.RestaurantId.ToString(), orderPayload);
        }
    }
}
