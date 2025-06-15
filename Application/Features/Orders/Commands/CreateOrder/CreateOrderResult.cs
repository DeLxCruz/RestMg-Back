namespace Application.Features.Orders.Commands.CreateOrder
{
    public record CreateOrderResult(Guid OrderId, string OrderCode);
}