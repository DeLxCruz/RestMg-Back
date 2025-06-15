using MediatR;

namespace Application.Features.Orders.Commands.CreateOrder
{
    public record CreateOrderCommand(
        Guid RestaurantId,
        Guid TableId,
        List<OrderItemCommandDto> Items
    ) : IRequest<CreateOrderResult>;
}