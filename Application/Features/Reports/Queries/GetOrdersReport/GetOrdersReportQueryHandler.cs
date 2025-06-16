using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Reports.Queries.GetOrdersReport
{
    public class GetOrdersReportQueryHandler(IApplicationDbContext db, ICurrentUserService user)
        : IRequestHandler<GetOrdersReportQuery, List<OrdersReportItemDto>>
    {
        public async Task<List<OrdersReportItemDto>> Handle(GetOrdersReportQuery request, CancellationToken ct)
        {
            var restaurantId = user.RestaurantId ?? throw new UnauthorizedAccessException();

            var query = db.Orders
                .AsNoTracking()
                .Where(o => o.RestaurantId == restaurantId);

            // Aplicar filtro de fecha "desde"
            if (request.From.HasValue)
            {
                query = query.Where(o => o.CreatedAt >= request.From.Value.ToUniversalTime());
            }

            // Aplicar filtro de fecha "hasta"
            if (request.To.HasValue)
            {
                query = query.Where(o => o.CreatedAt < request.To.Value.AddDays(1).ToUniversalTime());
            }

            var reportItems = await query
                .Include(o => o.Table)
                .OrderByDescending(o => o.CreatedAt) // Los más recientes primero
                .Select(o => new OrdersReportItemDto(
                    o.Id,
                    o.OrderCode,
                    o.Table.Code,
                    o.Total,
                    o.Status.ToString(),
                    o.CreatedAt
                ))
                .ToListAsync(ct);

            return reportItems;
        }
    }
}