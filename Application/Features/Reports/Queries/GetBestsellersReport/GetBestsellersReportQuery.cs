using MediatR;

namespace Application.Features.Reports.Queries.GetBestsellersReport
{
    public record GetBestsellersReportQuery(DateTime? From, DateTime? To)
        : IRequest<List<BestsellerReportItemDto>>;
}