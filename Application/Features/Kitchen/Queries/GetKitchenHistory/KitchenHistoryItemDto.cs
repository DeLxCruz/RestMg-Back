namespace Application.Features.Kitchen.Queries.GetKitchenHistory
{
    public record KitchenHistoryItemDto(
        string OrderCode,
        string TableCode,
        string Status,
        DateTime CreatedAt,
        DateTime? CompletedAt
    );
}