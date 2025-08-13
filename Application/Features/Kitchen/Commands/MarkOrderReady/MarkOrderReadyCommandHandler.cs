using Application.Common.Interfaces;
using Application.Common.Notifications;
using Domain.Enums;
using MediatR;

namespace Application.Features.Kitchen.Commands.MarkOrderReady
{
    public class MarkOrderReadyCommandHandler(IApplicationDbContext db, ICurrentUserService user, IPublisher publisher)
        : IRequestHandler<MarkOrderReadyCommand>
    {
        public async Task Handle(MarkOrderReadyCommand command, CancellationToken ct)
        {
            var restaurantId = user.RestaurantId ?? throw new UnauthorizedAccessException();
            var order = await db.Orders.FindAsync(command.Id) ?? throw new KeyNotFoundException("Pedido no encontrado.");

            if (order.RestaurantId != restaurantId) throw new UnauthorizedAccessException();
            if (order.Status != OrderStatus.InPreparation) throw new InvalidOperationException("Solo se puede marcar como listo un pedido que está en preparación.");

            order.Status = OrderStatus.Ready;
            order.CompletedAt = DateTime.UtcNow;
            
            await db.SaveChangesAsync(ct);

            // Publicar notificación
            await publisher.Publish(new OrderStatusChangedNotification(restaurantId, order.Id, order.Status.ToString()), ct);
        }
    }
}