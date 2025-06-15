using MediatR;

namespace Application.Common.Notifications
{
    public record MenuItemAvailabilityNotification(
        Guid RestaurantId,
        Guid MenuItemId,
        bool IsAvailable
    ) : INotification;
}