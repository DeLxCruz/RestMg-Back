using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Features.Orders.Queries.GetOrderByCode;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Features.Orders.Queries.GetRestaurantOrders
{
    public class GetRestaurantOrdersQueryHandler(IApplicationDbContext db)
        : IRequestHandler<GetRestaurantOrdersQuery, PagedList<OrderDetailDto>>
    {
        public async Task<PagedList<OrderDetailDto>> Handle(GetRestaurantOrdersQuery request, CancellationToken ct)
        {
            var query = db.Orders
                .AsNoTracking()
                .Where(o => o.RestaurantId == request.RestaurantId)
                .Include(o => o.Table)
                .Include(o => o.Items)
                .ThenInclude(oi => oi.MenuItem)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new OrderDetailDto(
                    o.Id,
                    o.Table.Code,
                    o.OrderCode,
                    o.Status.ToString(),
                    o.Total,
                    o.CreatedAt,
                    o.Items.Select(oi => new OrderItemDetailDto(oi.MenuItem.Name, oi.Quantity, oi.UnitPrice)).ToList()
                ));

            return await PagedList<OrderDetailDto>.CreateAsync(query, request.PageNumber, request.PageSize);
        }
    }
}
