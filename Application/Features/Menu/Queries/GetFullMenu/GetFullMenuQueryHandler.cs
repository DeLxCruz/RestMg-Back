using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Menu.Queries.GetFullMenu
{
    public class GetFullMenuQueryHandler(IApplicationDbContext db) : IRequestHandler<GetFullMenuQuery, List<MenuCategoryDto>>
    {
        public async Task<List<MenuCategoryDto>> Handle(GetFullMenuQuery request, CancellationToken ct)
        {
            return await db.Categories
                .AsNoTracking()
                .Where(c => c.RestaurantId == request.RestaurantId && c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .Select(c => new MenuCategoryDto(c.Id, c.Name, c.DisplayOrder,
                    c.Items
                        .Where(i => i.IsActive)
                        .OrderBy(i => i.Name)
                        .Select(i => new MenuItemDto(i.Id, i.Name, i.Description, i.Price, i.ImageUrl, i.IsAvailable))
                        .ToList()))
                .ToListAsync(ct);
        }
    }
}