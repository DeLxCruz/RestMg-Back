using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Branding.Queries.GetRestaurantLogo
{
    public class GetRestaurantLogoQueryHandler(IApplicationDbContext context)
        : IRequestHandler<GetRestaurantLogoQuery, string?>
    {
        public async Task<string?> Handle(GetRestaurantLogoQuery request, CancellationToken ct)
        {
            var restaurant = await context.Restaurants
                .Where(r => r.Id == request.RestaurantId)
                .Select(r => r.LogoUrl)
                .FirstOrDefaultAsync(ct);

            return restaurant;
        }
    }
}
