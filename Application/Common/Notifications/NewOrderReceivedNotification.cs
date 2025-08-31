using Domain.Models;
using MediatR;
using System;

namespace Application.Common.Notifications
{
    public record NewOrderReceivedNotification(
        Guid RestaurantId,
        Guid OrderId,
        string OrderCode,
        string Status,
        decimal Total,
        DateTime CreatedAt,
        string TableCode
    ) : INotification;
}
