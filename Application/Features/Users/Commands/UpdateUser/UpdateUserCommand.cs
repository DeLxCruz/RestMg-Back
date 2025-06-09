using MediatR;

namespace Application.Features.Users.Commands.UpdateUser
{
    public record UpdateUserCommand(
        Guid Id,
        string FullName,
        string Email,
        string Role,
        bool IsActive
    ) : IRequest;
}