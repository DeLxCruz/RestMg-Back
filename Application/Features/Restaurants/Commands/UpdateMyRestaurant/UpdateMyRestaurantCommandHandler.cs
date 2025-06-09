using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Restaurants.Commands.UpdateMyRestaurant
{
    public class UpdateMyRestaurantCommandHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService) : IRequestHandler<UpdateMyRestaurantCommand>
    {
        public async Task Handle(UpdateMyRestaurantCommand command, CancellationToken ct)
        {
            var restaurantId = currentUserService.RestaurantId ?? throw new UnauthorizedAccessException();

            var restaurant = await dbContext.Restaurants.FindAsync(restaurantId)
                ?? throw new Exception("Restaurante no encontrado.");

            restaurant.Name = command.Name;
            restaurant.BrandingColor = command.BrandingColor;
            restaurant.LogoUrl = command.LogoUrl;

            await dbContext.SaveChangesAsync(ct);
        }
    }
}