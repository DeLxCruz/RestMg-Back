namespace Application.Features.Orders.Commands.CreateOrder
{
    public record OrderItemCommandDto(Guid MenuItemId, int Quantity);
}