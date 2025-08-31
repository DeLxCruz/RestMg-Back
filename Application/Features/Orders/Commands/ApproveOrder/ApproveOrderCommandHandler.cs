using Application.Common.Interfaces;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Orders.Commands.ApproveOrder
{
    public class ApproveOrderCommandHandler(IApplicationDbContext db, IPublisher publisher) : IRequestHandler<ApproveOrderCommand>
    {
        public async Task Handle(ApproveOrderCommand request, CancellationToken ct)
        {
            var order = await db.Orders.FirstOrDefaultAsync(o => o.Id == request.OrderId, ct)
                ?? throw new KeyNotFoundException("Order not found");

            if (order.Status != OrderStatus.AwaitingPayment)
            {
                throw new InvalidOperationException("Only orders awaiting payment can be approved.");
            }

            order.Status = OrderStatus.Pending;
            await db.SaveChangesAsync(ct);

            Console.WriteLine($"ApproveOrderCommandHandler: Publishing OrderStatusChangedNotification for OrderId: {order.Id}, RestaurantId: {order.RestaurantId}, NewStatus: {order.Status}");
            // Publicar notificación de cambio de estado
            await publisher.Publish(new Application.Common.Notifications.OrderStatusChangedNotification(order.RestaurantId, order.Id, order.Status.ToString()), ct);
        }
    }
}
