namespace API.DTOs
{
    public record UpdateRestaurantRequest(
        string Name,
        string? BrandingColor,
        string? LogoUrl
    );
}