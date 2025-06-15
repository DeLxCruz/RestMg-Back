using Application.Features.Menu.Queries.GetFullMenu;
using MediatR;

namespace Application.Features.Menu.Queries.GetMenuItemDetail
{
    public record GetMenuItemDetailQuery(Guid ItemId) : IRequest<MenuItemDto>;
}