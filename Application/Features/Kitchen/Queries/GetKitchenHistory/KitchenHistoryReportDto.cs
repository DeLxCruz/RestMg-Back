namespace Application.Features.Kitchen.Queries.GetKitchenHistory
{
    public record KitchenHistoryReportDto(
          List<KitchenHistoryItemDto> Orders,
          int TotalCompletedOrders,
          double AveragePreparationTimeMinutes
    );
}