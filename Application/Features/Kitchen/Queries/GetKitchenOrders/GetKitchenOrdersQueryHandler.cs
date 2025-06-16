using Application.Common.Interfaces;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Kitchen.Queries.GetKitchenOrders
{
    public class GetKitchenOrdersQueryHandler(IApplicationDbContext db, ICurrentUserService user)
        : IRequestHandler<GetKitchenOrdersQuery, List<KitchenOrderDto>>
    {
        public async Task<List<KitchenOrderDto>> Handle(GetKitchenOrdersQuery request, CancellationToken ct)
        {
            var restaurantId = user.RestaurantId ?? throw new UnauthorizedAccessException();

            var query = db.Orders
                .AsNoTracking()
                .Where(o => o.RestaurantId == restaurantId
                            && o.Status != OrderStatus.AwaitingPayment
                            && o.Status != OrderStatus.Delivered
                            && o.Status != OrderStatus.Cancelled);

            if (Enum.TryParse<OrderStatus>(request.Status, true, out var filterStatus))
            {
                query = query.Where(o => o.Status == filterStatus);
            }

            var orders = await query
                .Include(o => o.Table)
                .Include(o => o.Items)
                .ThenInclude(oi => oi.MenuItem)
                .OrderBy(o => o.CreatedAt)
                .Select(o => new KitchenOrderDto(
                    o.Id,
                    o.OrderCode,
                    o.Table.Code,
                    o.Status,
                    o.CreatedAt,
                    o.Items.Select(oi => new KitchenOrderItemDto(oi.MenuItem.Name, oi.Quantity)).ToList()
                ))
                .ToListAsync(ct);

            return orders;
        }
    }
}