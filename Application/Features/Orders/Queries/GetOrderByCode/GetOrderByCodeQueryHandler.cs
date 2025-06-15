using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Orders.Queries.GetOrderByCode
{
    public class GetOrderByCodeQueryHandler(IApplicationDbContext db)
    : IRequestHandler<GetOrderByCodeQuery, OrderDetailDto>
    {
        public async Task<OrderDetailDto> Handle(GetOrderByCodeQuery request, CancellationToken ct)
        {
            var order = await db.Orders
                .AsNoTracking()
                .Include(o => o.Items)
                .ThenInclude(oi => oi.MenuItem) // Para obtener el nombre del plato
                .Where(o => o.RestaurantId == request.RestaurantId && o.OrderCode == request.OrderCode)
                .Select(o => new OrderDetailDto(
                    o.Id,
                    o.OrderCode,
                    o.Status.ToString(),
                    o.Total,
                    o.CreatedAt,
                    o.Items.Select(oi => new OrderItemDetailDto(oi.MenuItem.Name, oi.Quantity, oi.UnitPrice)).ToList()
                ))
                .FirstOrDefaultAsync(ct);

            return order ?? throw new KeyNotFoundException("Pedido no encontrado.");
        }
    }
}