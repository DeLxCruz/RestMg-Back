namespace Application.Features.Reports.Queries.GetOrdersReport
{
    public record OrdersReportItemDto(
        Guid Id,
        string OrderCode,
        string TableCode,
        decimal Total,
        string Status,
        DateTime CreatedAt
    );
}