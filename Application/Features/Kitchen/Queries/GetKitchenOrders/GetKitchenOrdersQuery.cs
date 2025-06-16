using MediatR;

namespace Application.Features.Kitchen.Queries.GetKitchenOrders
{
    public record GetKitchenOrdersQuery(string? Status) : IRequest<List<KitchenOrderDto>>;
}