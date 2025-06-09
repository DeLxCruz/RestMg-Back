using MediatR;

namespace Application.Features.Users.Commands.CreateUser
{
    public record CreateUserCommand(
        string FullName,
        string Email,
        string Password,
        string Role
    ) : IRequest<Guid>;
}