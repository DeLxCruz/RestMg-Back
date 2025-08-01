using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Branding.Commands.UpdateRestaurantLogo
{
    public record UpdateRestaurantLogoCommand(Guid RestaurantId, IFormFile File) : IRequest<string>;
}
