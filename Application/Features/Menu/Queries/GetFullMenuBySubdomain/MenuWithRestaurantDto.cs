using Application.Features.Menu.Queries.GetFullMenu;

namespace Application.Features.Menu.Queries.GetFullMenuBySubdomain
{
    public class MenuWithRestaurantDto
    {
        public Guid RestaurantId { get; set; }
        public List<MenuCategoryDto> Categories { get; set; } = new();
    }
}
