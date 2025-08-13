namespace Application.Features.Dashboard.Queries.GetDashboardSummary
{
    public record DashboardSummaryDto(
        decimal RevenueToday,
        int OrdersToday,
        decimal AverageTicketToday
    );
}