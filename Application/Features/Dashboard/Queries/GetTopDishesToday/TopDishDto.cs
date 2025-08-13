namespace Application.Features.Dashboard.Queries.GetTopDishesToday
{
    public record TopDishDto(
        string Name,
        int TotalSold
    );
}