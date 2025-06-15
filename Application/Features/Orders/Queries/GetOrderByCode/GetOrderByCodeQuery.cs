using MediatR;

namespace Application.Features.Orders.Queries.GetOrderByCode
{
    public record GetOrderByCodeQuery(Guid RestaurantId, string OrderCode) : IRequest<OrderDetailDto>;
}