using MediatR;

namespace Application.Features.Kitchen.Notifications
{
    public record NewOrderReceivedNotification(
        Guid RestaurantId,
        Guid OrderId,
        string TableCode,
        List<OrderItemNotificationDto> Items,
        DateTime CreatedAt
    ) : INotification;
}