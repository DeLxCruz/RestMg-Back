using MediatR;

namespace Application.Features.MenuItems.Commands.DeleteMenuItem
{
    public record DeleteMenuItemCommand(Guid Id) : IRequest;
}