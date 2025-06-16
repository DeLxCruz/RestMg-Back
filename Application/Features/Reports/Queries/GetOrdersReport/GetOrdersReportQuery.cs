using MediatR;

namespace Application.Features.Reports.Queries.GetOrdersReport
{
    public record GetOrdersReportQuery(DateTime? From, DateTime? To)
        : IRequest<List<OrdersReportItemDto>>;
}