using MediatR;

namespace Application.Features.Branding.Queries.GetRestaurantLogo
{
    public record GetRestaurantLogoQuery(Guid RestaurantId) : IRequest<string?>;
}
