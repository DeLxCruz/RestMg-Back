
using Application.Common.Interfaces;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Orders.Commands.DeliverOrder
{
    public class DeliverOrderCommandHandler(IApplicationDbContext db, ICurrentUserService user) 
        : IRequestHandler<DeliverOrderCommand>
    {
        public async Task Handle(DeliverOrderCommand request, CancellationToken ct)
        {
            var restaurantId = user.RestaurantId ?? throw new UnauthorizedAccessException();

            var order = await db.Orders.FirstOrDefaultAsync(o => o.Id == request.OrderId, ct)
                ?? throw new KeyNotFoundException("Pedido no encontrado.");

            if (order.RestaurantId != restaurantId) throw new UnauthorizedAccessException("No tienes permiso para modificar este pedido.");

            if (order.Status != OrderStatus.Ready)
            {
                throw new InvalidOperationException("Solo se pueden marcar como entregados los pedidos que están listos.");
            }

            order.Status = OrderStatus.Delivered;
            order.CompletedAt = DateTime.UtcNow;

            await db.SaveChangesAsync(ct);
        }
    }
}
