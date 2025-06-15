using MediatR;

namespace Application.Features.MenuItems.Commands.CreateMenuItem
{
    public record CreateMenuItemCommand(Guid CategoryId, string Name, string? Description, decimal Price, string? ImageUrl) : IRequest<Guid>;
}