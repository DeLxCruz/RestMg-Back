namespace Application.Features.Tables.Queries.GetTables
{
    public record TableDto(
        Guid Id,
        string Code,
        bool IsActive
    );
}