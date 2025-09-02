namespace API.DTOs
{
    public record UpdateRestaurantRequest(
        string Name,
        string? BrandingColor,
        string? LogoUrl,
        string? BannerUrl,
        string? ClientUrl,
        string? Subdomain
    );
}