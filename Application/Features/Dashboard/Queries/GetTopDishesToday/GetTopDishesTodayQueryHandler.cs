using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Dashboard.Queries.GetTopDishesToday
{
    public class GetTopDishesTodayQueryHandler(IApplicationDbContext db, ICurrentUserService user)
        : IRequestHandler<GetTopDishesTodayQuery, List<TopDishDto>>
    {
        public async Task<List<TopDishDto>> Handle(GetTopDishesTodayQuery request, CancellationToken ct)
        {
            var restaurantId = user.RestaurantId ?? throw new UnauthorizedAccessException();
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            var hasOrderItemsToday = await db.OrderItems
                .AnyAsync(oi => oi.Order.RestaurantId == restaurantId
                                && oi.Order.CreatedAt >= today
                                && oi.Order.CreatedAt < tomorrow, ct);

            if (!hasOrderItemsToday)
            {
                return new List<TopDishDto>();
            }

            var topDishes = await db.OrderItems
                .AsNoTracking()
                .Where(oi => oi.Order.RestaurantId == restaurantId
                            && oi.Order.CreatedAt >= today
                            && oi.Order.CreatedAt < tomorrow)
                .GroupBy(oi => new { oi.MenuItem.Name })
                .Select(g => new TopDishDto(
                    g.Key.Name,
                    g.Sum(oi => oi.Quantity)
                ))
                .OrderByDescending(d => d.TotalSold)
                .Take(3)
                .ToListAsync(ct);

            return topDishes;
        }
    }
}