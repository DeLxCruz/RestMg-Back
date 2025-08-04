namespace Application.Features.Restaurants.Queries.GetMyRestaurant
{
    public record MyRestaurantDto(
        Guid Id,
        string Name,
        string? BrandingColor,
        string? LogoUrl,
        string? Subdomain
    );
}