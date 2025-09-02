using MediatR;

namespace Application.Features.Restaurants.Commands.UpdateMyRestaurant
{
    public record UpdateMyRestaurantCommand(
        string Name,
        string? BrandingColor,
        string? LogoUrl,
        string? BannerUrl,
        string? ClientUrl,
        string? Subdomain
    ) : IRequest;
}