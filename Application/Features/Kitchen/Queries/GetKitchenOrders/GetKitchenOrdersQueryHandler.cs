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
            var restaurantId = user.RestaurantId ?? throw new System.UnauthorizedAccessException();

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

            var queryWithIncludes = query
                .Include(o => o.Table)
                .Include(o => o.Items)
                .ThenInclude(oi => oi.MenuItem);

            var orderedQuery = queryWithIncludes.OrderByDescending(o => o.CreatedAt);

            IQueryable<Domain.Models.Order> finalQuery = orderedQuery;
            if (request.Limit.HasValue && request.Limit > 0)
            {
                finalQuery = orderedQuery.Take(request.Limit.Value);
            }

            var orders = await finalQuery
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