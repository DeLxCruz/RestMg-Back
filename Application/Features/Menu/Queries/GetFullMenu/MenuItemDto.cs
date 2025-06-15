namespace Application.Features.Menu.Queries.GetFullMenu
{
    public record MenuItemDto(Guid Id, string Name, string? Description, decimal Price, string? ImageUrl, bool IsAvailable);
}