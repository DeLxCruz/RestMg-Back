using Domain.Enums;

namespace Application.Features.Kitchen.Queries.GetKitchenOrders
{
    public record KitchenOrderDto(
        Guid Id,
        string OrderCode,
        string TableCode,
        OrderStatus Status,
        DateTime CreatedAt,
        List<KitchenOrderItemDto> Items,
        decimal TotalPrice
    );
}