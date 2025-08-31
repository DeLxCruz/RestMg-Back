using Application.Common.Interfaces;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Dashboard.Queries.GetDashboardSummary
{
    public class GetDashboardSummaryQueryHandler(IApplicationDbContext db, ICurrentUserService user)
        : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
    {
        public async Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery request, CancellationToken ct)
        {
            var restaurantId = user.RestaurantId ?? throw new UnauthorizedAccessException();
            var colombiaZone = TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time");
            var colombiaTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, colombiaZone);
            var today = colombiaTime.Date;
            var tomorrow = today.AddDays(1);

            var ordersToday = await db.Orders
                .AsNoTracking()
                .Where(o => o.RestaurantId == restaurantId
                            && o.CreatedAt >= today
                            && o.CreatedAt < tomorrow
                            && o.Status == OrderStatus.Delivered)
                .ToListAsync(ct);

            var revenueToday = ordersToday.Sum(o => o.Total);
            var ordersCount = ordersToday.Count;
            var averageTicket = ordersCount > 0 ? revenueToday / ordersCount : 0;

            return new DashboardSummaryDto(revenueToday, ordersCount, averageTicket);
        }
    }
}