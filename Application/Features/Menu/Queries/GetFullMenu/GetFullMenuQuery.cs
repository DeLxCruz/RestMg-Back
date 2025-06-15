using MediatR;

namespace Application.Features.Menu.Queries.GetFullMenu
{
    public record GetFullMenuQuery(Guid RestaurantId) : IRequest<List<MenuCategoryDto>>;
}
