using MediatR;

namespace Application.Features.Dashboard.Queries.GetDashboardSummary
{
    public record GetDashboardSummaryQuery : IRequest<DashboardSummaryDto>;
}