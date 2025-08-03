using System.Text.RegularExpressions;
using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Restaurants.Commands.UpdateMyRestaurant
{
    public partial class UpdateMyRestaurantCommandHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService) : IRequestHandler<UpdateMyRestaurantCommand>
    {
        public async Task Handle(UpdateMyRestaurantCommand command, CancellationToken ct)
        {
            var restaurantId = currentUserService.RestaurantId ?? throw new UnauthorizedAccessException();

            var restaurant = await dbContext.Restaurants.FindAsync(restaurantId)
                ?? throw new Exception("Restaurante no encontrado.");

            // Validar que el subdominio no esté en uso por otro restaurante y tennga un formato válido
            if (!string.IsNullOrWhiteSpace(command.Subdomain))
            {
                var formattedSubdomain = command.Subdomain.ToLowerInvariant().Trim();

                var subdomainExists = await dbContext.Restaurants
                .AnyAsync(r => r.Subdomain == formattedSubdomain && r.Id != restaurantId, ct);

                if (subdomainExists)
                {
                    throw new InvalidOperationException("Este subdominio ya está en uso por otro restaurante.");
                }

                // Validar formato del subdominio
                if (!MyRegex().IsMatch(formattedSubdomain))
                {
                    throw new Exception("El subdominio debe contener solo letras minúsculas, números y guiones.");
                }

                restaurant.Subdomain = formattedSubdomain;
            }

            restaurant.Name = command.Name;
            restaurant.BrandingColor = command.BrandingColor;
            restaurant.LogoUrl = command.LogoUrl;
            restaurant.ClientUrl = command.ClientUrl;

            await dbContext.SaveChangesAsync(ct);
        }

        [GeneratedRegex(@"^[a-z0-9]+(-[a-z0-9]+)*$")]
        private static partial Regex MyRegex();
    }
}