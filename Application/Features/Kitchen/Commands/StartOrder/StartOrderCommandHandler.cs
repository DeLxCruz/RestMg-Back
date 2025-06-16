using Application.Common.Interfaces;
using Application.Common.Notifications;
using Domain.Enums;
using MediatR;

namespace Application.Features.Kitchen.Commands.StartOrder
{
    public class StartOrderCommandHandler(IApplicationDbContext db, ICurrentUserService user, IPublisher publisher)
        : IRequestHandler<StartOrderCommand>
    {
        public async Task Handle(StartOrderCommand command, CancellationToken ct)
        {
            var restaurantId = user.RestaurantId ?? throw new UnauthorizedAccessException();
            var order = await db.Orders.FindAsync(command.Id) ?? throw new KeyNotFoundException("Pedido no encontrado.");

            if (order.RestaurantId != restaurantId) throw new UnauthorizedAccessException();
            if (order.Status != OrderStatus.Pending) throw new InvalidOperationException("Solo se puede iniciar un pedido que está pendiente.");

            order.Status = OrderStatus.InPreparation;
            await db.SaveChangesAsync(ct);

            // Publicar notificación
            await publisher.Publish(new OrderStatusChangedNotification(restaurantId, order.Id, order.Status.ToString()), ct);
        }
    }
}