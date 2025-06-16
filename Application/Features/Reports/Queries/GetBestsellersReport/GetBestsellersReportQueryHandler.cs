using Application.Common.Interfaces;
using Domain.Enums;
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

            var query = db.OrderItems
                .AsNoTracking()
                .Where(oi => oi.Order.RestaurantId == restaurantId);

            // Se filtran los pedidos por fecha
            if (request.From.HasValue)
            {
                query = query.Where(oi => oi.Order.CreatedAt >= request.From.Value.ToUniversalTime());
            }
            if (request.To.HasValue)
            {
                query = query.Where(oi => oi.Order.CreatedAt < request.To.Value.AddDays(1).ToUniversalTime());
            }

            var bestsellers = await query
                .GroupBy(oi => oi.MenuItem)
                .Select(group => new BestsellerReportItemDto(
                    group.Key.Id,
                    group.Key.Name,
                    group.Sum(oi => oi.Quantity),
                    group.Sum(oi => oi.Quantity * oi.UnitPrice)
                ))
                .OrderByDescending(dto => dto.TotalSold) // Ordenar por el más vendido
                .ThenByDescending(dto => dto.TotalRevenue)
                .Take(10) // Tomar, por ejemplo, el Top 10
                .ToListAsync(ct);

            return bestsellers;
        }
    }
}