using MediatR;

namespace Application.Common.Notifications
{
    public record TableStateChangedNotification(
        Guid RestaurantId,
        Guid TableId,
        string NewState
    ) : INotification;
}