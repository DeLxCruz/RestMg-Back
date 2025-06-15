using MediatR;

namespace Application.Features.MenuItems.Commands.UpdateMenuItem
{
    public record UpdateMenuItemCommand(Guid Id, Guid CategoryId, string Name, string? Description, decimal Price, string? ImageUrl, bool IsAvailable) : IRequest;
}