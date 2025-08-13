using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Reports.Queries.GetBestsellersReport
{
    public class GetBestsellersReportQueryHandler(IApplicationDbContext db, ICurrentUserService user)
        : IRequestHandler<GetBestsellersReportQuery, List<BestsellerReportItemDto>>
    {
        public async Task<List<BestsellerReportItemDto>> Handle(GetBestsellersReportQuery request, CancellationToken ct)
        {
            var restaurantId = user.RestaurantId ?? throw new UnauthorizedAccessException();

            var hasOrderItems = await db.OrderItems
                .AnyAsync(oi => oi.Order.RestaurantId == restaurantId, ct);

            if (!hasOrderItems)
            {
                return new List<BestsellerReportItemDto>();
            }

            var query = db.OrderItems
                .AsNoTracking()
                .Where(oi => oi.Order.RestaurantId == restaurantId);

            if (request.From.HasValue)
            {
                query = query.Where(oi => oi.Order.CreatedAt >= request.From.Value.ToUniversalTime());
            }
            if (request.To.HasValue)
            {
                query = query.Where(oi => oi.Order.CreatedAt < request.To.Value.AddDays(1).ToUniversalTime());
            }

            var salesData = await query
                .GroupBy(oi => oi.MenuItemId)
                .Select(group => new
                {
                    MenuItemId = group.Key,
                    TotalSold = group.Sum(oi => oi.Quantity),
                    TotalRevenue = group.Sum(oi => oi.Quantity * oi.UnitPrice)
                })
                .OrderByDescending(x => x.TotalSold)
                .ThenByDescending(x => x.TotalRevenue)
                .Take(10)
                .ToListAsync(ct);

            if (!salesData.Any())
            {
                return new List<BestsellerReportItemDto>();
            }

            var menuItemIds = salesData.Select(s => s.MenuItemId).ToList();
            var menuItems = await db.MenuItems
                .AsNoTracking()
                .Where(mi => menuItemIds.Contains(mi.Id))
                .ToDictionaryAsync(mi => mi.Id, mi => mi.Name, ct);

            var bestsellers = salesData.Select(s => new BestsellerReportItemDto(
                s.MenuItemId,
                menuItems.GetValueOrDefault(s.MenuItemId, "Plato Eliminado"),
                s.TotalSold,
                s.TotalRevenue
            )).ToList();

            return bestsellers;
        }
    }
}