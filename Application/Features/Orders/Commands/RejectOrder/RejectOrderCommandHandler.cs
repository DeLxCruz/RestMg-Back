using Application.Common.Interfaces;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Orders.Commands.RejectOrder
{
    public class RejectOrderCommandHandler(IApplicationDbContext db) : IRequestHandler<RejectOrderCommand>
    {
        public async Task Handle(RejectOrderCommand request, CancellationToken ct)
        {
            var order = await db.Orders
                .Include(o => o.Table) // Incluir la mesa para poder actualizarla
                .FirstOrDefaultAsync(o => o.Id == request.OrderId, ct)
                ?? throw new KeyNotFoundException("Pedido no encontrado.");

            if (order.Status != OrderStatus.AwaitingPayment)
            {
                throw new InvalidOperationException("Solo se puede rechazar un pedido que está esperando pago.");
            }

            // Cambiar el estado del pedido a Cancelled
            order.Status = OrderStatus.Cancelled;

            // Liberar la mesa, ya que el pedido no procederá
            if (order.Table != null)
            {
                order.Table.Status = TableStatus.Available;
            }

            await db.SaveChangesAsync(ct);
        }
    }
}
