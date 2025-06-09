using Application.Features.Auth.Commands.Login;
using MediatR;

namespace Application.Features.Auth.Commands.RefreshToken
{
    public record RefreshTokenCommand : IRequest<AuthResult>;
}