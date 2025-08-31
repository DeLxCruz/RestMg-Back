using Domain.Models;
using MediatR;

namespace Application.Features.Kitchen.Notifications
{
    public record NewOrderForKitchenNotification(
        Guid RestaurantId,
        Guid OrderId,
        string TableCode,
        List<OrderItemNotificationDto> Items,
        DateTime CreatedAt
    ) : INotification;
}
