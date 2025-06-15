namespace Application.Features.Tables.Queries.GetTables
{
    public record TableDto(
        Guid Id,
        string Code,
        string Status
    );
}