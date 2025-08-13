using MediatR;

namespace Application.Features.Kitchen.Queries.GetKitchenOrders
{
    public record GetKitchenOrdersQuery(string? Status, int? Limit) : IRequest<List<KitchenOrderDto>>;
}