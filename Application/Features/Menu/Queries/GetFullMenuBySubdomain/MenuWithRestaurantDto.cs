using Application.Features.Menu.Queries.GetFullMenu;

namespace Application.Features.Menu.Queries.GetFullMenuBySubdomain
{
    public class MenuWithRestaurantDto
    {
        public Guid RestaurantId { get; set; }
        public string RestaurantName { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string? BannerUrl { get; set; }
        public string? BrandingColor { get; set; }
        public List<MenuCategoryDto> Categories { get; set; } = new();
    }
}
