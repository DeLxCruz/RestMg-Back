using MediatR;

namespace Application.Features.Kitchen.Queries.GetKitchenHistory
{
    public record GetKitchenHistoryQuery : IRequest<KitchenHistoryReportDto>;
}