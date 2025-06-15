using Application.Common.Interfaces;
using Application.Features.Menu.Queries.GetFullMenu;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Menu.Queries.GetMenuItemDetail
{
    public class GetMenuItemDetailQueryHandler(IApplicationDbContext dbContext)
        : IRequestHandler<GetMenuItemDetailQuery, MenuItemDto>
    {
        public async Task<MenuItemDto> Handle(GetMenuItemDetailQuery request, CancellationToken ct)
        {
            var menuItem = await dbContext.MenuItems
                .AsNoTracking()
                .Where(i => i.Id == request.ItemId && i.IsActive) // Validar que existe Y está activo
                .Select(i => new MenuItemDto(
                    i.Id,
                    i.Name,
                    i.Description,
                    i.Price,
                    i.ImageUrl,
                    i.IsAvailable))
                .FirstOrDefaultAsync(ct);

            // Si no se encuentra o no está activo, se devuelve null. El handler lanzará una excepción.
            return menuItem ?? throw new KeyNotFoundException("Plato no encontrado o no está disponible en el menú.");
        }
    }
}