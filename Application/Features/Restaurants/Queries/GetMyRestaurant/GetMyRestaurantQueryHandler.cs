using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Restaurants.Queries.GetMyRestaurant
{
    public class GetMyRestaurantQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService) : IRequestHandler<GetMyRestaurantQuery, MyRestaurantDto>
    {
        public async Task<MyRestaurantDto> Handle(GetMyRestaurantQuery request, CancellationToken ct)
        {
            var restaurantId = currentUserService.RestaurantId ?? throw new UnauthorizedAccessException();

            var restaurant = await dbContext.Restaurants
                .AsNoTracking()
                .Where(r => r.Id == restaurantId)
                .Select(r => new MyRestaurantDto(r.Id, r.Name, r.BrandingColor, r.LogoUrl))
                .FirstOrDefaultAsync(ct);

            return restaurant ?? throw new Exception("Restaurante no encontrado.");
        }
    }
}