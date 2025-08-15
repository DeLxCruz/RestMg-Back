using Application.Common.Interfaces;
using Application.Features.Orders.Queries.GetOrderByCode;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Orders.Queries.GetRestaurantOrders
{
    public class GetRestaurantOrdersQueryHandler(IApplicationDbContext db)
        : IRequestHandler<GetRestaurantOrdersQuery, List<OrderDetailDto>>
    {
        public async Task<List<OrderDetailDto>> Handle(GetRestaurantOrdersQuery request, CancellationToken ct)
        {
            var orders = await db.Orders
                .AsNoTracking()
                .Where(o => o.RestaurantId == request.RestaurantId)
                .Include(o => o.Items)
                .ThenInclude(oi => oi.MenuItem)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderDetailDto(
                    o.Id,
                    o.OrderCode,
                    o.Status.ToString(),
                    o.Total,
                    o.CreatedAt,
                    o.Items.Select(oi => new OrderItemDetailDto(oi.MenuItem.Name, oi.Quantity, oi.UnitPrice)).ToList()
                ))
                .ToListAsync(ct);

            return orders;
        }
    }
}
