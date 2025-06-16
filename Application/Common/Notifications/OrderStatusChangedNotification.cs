using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Application.Common.Notifications
{
    public record OrderStatusChangedNotification(
        Guid RestaurantId,
        Guid OrderId,
        string NewStatus
    ) : INotification;
}