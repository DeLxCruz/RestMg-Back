namespace Application.Features.Menu.Queries.GetFullMenu
{
    public record MenuCategoryDto(Guid Id, string Name, int DisplayOrder, List<MenuItemDto> Items);
}