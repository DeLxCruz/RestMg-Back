using Application.Common.Interfaces;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
namespace Application.Features.Kitchen.Queries.GetKitchenHistory
{
    public class GetKitchenHistoryQueryHandler(IApplicationDbContext db, ICurrentUserService user)
        : IRequestHandler<GetKitchenHistoryQuery, KitchenHistoryReportDto>
    {
        public async Task<KitchenHistoryReportDto> Handle(GetKitchenHistoryQuery request, CancellationToken ct)
        {
            var restaurantId = user.RestaurantId ?? throw new UnauthorizedAccessException();
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            var ordersToday = await db.Orders
                .AsNoTracking()
                .Include(o => o.Table)
                .Where(o => o.RestaurantId == restaurantId
                            && o.CreatedAt >= today
                            && o.CreatedAt < tomorrow
                            && (o.Status == OrderStatus.Delivered || o.Status == OrderStatus.Ready)) // Solo los completados
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync(ct);

            var orderDtos = ordersToday
                .Select(o => new KitchenHistoryItemDto(
                    o.OrderCode,
                    o.Table.Code,
                    o.Status.ToString(),
                    o.CreatedAt,
                    o.CompletedAt
                )).ToList();

            var totalCompleted = ordersToday.Count;

            // --- CÁLCULO DEL TIEMPO PROMEDIO ---
            double avgTimeMinutes = 0;
            if (totalCompleted > 0)
            {
                // Se filtran solo los pedidos que tienen una fecha de completado
                var ordersWithCompletionTime = ordersToday.Where(o => o.CompletedAt.HasValue).ToList();

                if (ordersWithCompletionTime.Any())
                {
                    // Se calcula la duración total en minutos para cada pedido y luego el promedio
                    avgTimeMinutes = ordersWithCompletionTime
                        .Average(o => (o.CompletedAt!.Value - o.CreatedAt).TotalMinutes);
                }
            }

            var report = new KitchenHistoryReportDto(orderDtos, totalCompleted, Math.Round(avgTimeMinutes, 2));

            return report;
        }
    }
}