using Application.Common.Interfaces;
using Application.Features.Menu.Queries.GetFullMenu;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Menu.Queries.GetFullMenuBySubdomain
{
    public class GetFullMenuBySubdomainQueryHandler(IApplicationDbContext dbContext)
        : IRequestHandler<GetFullMenuBySubdomainQuery, MenuWithRestaurantDto?>
    {
        public async Task<MenuWithRestaurantDto?> Handle(GetFullMenuBySubdomainQuery request, CancellationToken ct)
        {
            // Buscamos el restaurante por su subdominio, asegurándonos de que no sea sensible a mayúsculas/minúsculas.
            var restaurant = await dbContext.Restaurants
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Subdomain == request.Subdomain.ToLowerInvariant(), ct);

            // Si no se encuentra el restaurante, devolvemos una lista vacía.
            // La API lo traducirá en un 404 Not Found.
            if (restaurant == null)
            {
                return null;
            }

            // Si se encuentra, se obtienen sus categorías y platos activos.
            var categories = await dbContext.Categories
                .AsNoTracking()
                .Where(c => c.RestaurantId == restaurant.Id && c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .Select(c => new MenuCategoryDto(
                    c.Id,
                    c.Name,
                    c.DisplayOrder,
                    c.Items
                        .Where(i => i.IsActive)
                        .OrderBy(i => i.Name)
                        .Select(i => new MenuItemDto(i.Id, i.Name, i.Description, i.Price, i.ImageUrl, i.IsAvailable))
                        .ToList()
                ))
                .ToListAsync(ct);

            return new MenuWithRestaurantDto
            {
                RestaurantId = restaurant.Id,
                Categories = categories
            };
        }
    }
}