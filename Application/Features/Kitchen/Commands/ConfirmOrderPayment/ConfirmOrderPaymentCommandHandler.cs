using Application.Common.Interfaces;
using Application.Features.Kitchen.Notifications;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Kitchen.Commands.ConfirmOrderPayment
{
    public class ConfirmOrderPaymentCommandHandler(IApplicationDbContext db, ICurrentUserService user, IPublisher publisher)
        : IRequestHandler<ConfirmOrderPaymentCommand>
    {
        public async Task Handle(ConfirmOrderPaymentCommand command, CancellationToken ct)
        {
            var restaurantId = user.RestaurantId ?? throw new UnauthorizedAccessException();
            var order = await db.Orders
                .Include(o => o.Table)
                .Include(o => o.Items).ThenInclude(oi => oi.MenuItem)
                .FirstOrDefaultAsync(o => o.Id == command.OrderId, ct)
                ?? throw new KeyNotFoundException("Pedido no encontrado.");

            if (order.RestaurantId != restaurantId) throw new UnauthorizedAccessException();
            if (order.Status != OrderStatus.AwaitingPayment)
                throw new InvalidOperationException("Este pedido no está esperando pago.");

            // Cambiar el estado del pedido a Pendiente para la cocina
            order.Status = OrderStatus.Pending;
            await db.SaveChangesAsync(ct);

            Console.WriteLine($"[ConfirmOrderPaymentCommandHandler] ===== Publicando notificaciones =====");
            Console.WriteLine($"[ConfirmOrderPaymentCommandHandler] OrderId: {order.Id}");
            Console.WriteLine($"[ConfirmOrderPaymentCommandHandler] RestaurantId: {restaurantId}");
            Console.WriteLine($"[ConfirmOrderPaymentCommandHandler] TableCode: {order.Table.Code}");

            // Notificar a la cocina que tiene una nueva comanda
            var itemsDto = order.Items.Select(oi => new OrderItemNotificationDto(oi.MenuItem.Name, oi.Quantity)).ToList();
            Console.WriteLine($"[ConfirmOrderPaymentCommandHandler] Publicando NewOrderForKitchenNotification...");
            await publisher.Publish(new NewOrderForKitchenNotification(restaurantId, order.Id, order.Table.Code, itemsDto, order.CreatedAt), ct);

            // Notificar que el estado de la mesa ahora es (o sigue siendo) ocupado.
            Console.WriteLine($"[ConfirmOrderPaymentCommandHandler] Publicando TableStateChangedNotification...");
            await publisher.Publish(new Application.Common.Notifications.TableStateChangedNotification(restaurantId, order.TableId, order.Table.Status.ToString()), ct);
            Console.WriteLine($"[ConfirmOrderPaymentCommandHandler] ✅ Notificaciones publicadas exitosamente");
        }
    }
}