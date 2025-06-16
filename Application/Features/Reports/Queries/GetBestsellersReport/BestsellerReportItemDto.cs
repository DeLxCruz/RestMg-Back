namespace Application.Features.Reports.Queries.GetBestsellersReport
{
    public record BestsellerReportItemDto(
        Guid MenuItemId,
        string Name,
        int TotalSold,
        decimal TotalRevenue
    );
}