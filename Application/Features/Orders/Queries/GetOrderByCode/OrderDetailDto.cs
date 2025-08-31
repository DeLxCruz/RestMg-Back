namespace Application.Features.Orders.Queries.GetOrderByCode
{
    public record OrderDetailDto(Guid Id, string TableCode, string OrderCode, string Status, decimal Total, DateTime CreatedAt, List<OrderItemDetailDto> Items);
}