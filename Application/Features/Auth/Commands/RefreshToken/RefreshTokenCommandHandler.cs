using Application.Common.Interfaces;
using Application.Features.Auth.Commands.Login;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Auth.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IJwtTokenGenerator jwtTokenGenerator) : IRequestHandler<RefreshTokenCommand, AuthResult>
    {
        public async Task<AuthResult> Handle(RefreshTokenCommand request, CancellationToken ct)
        {
            var userId = currentUserService.UserId ?? throw new UnauthorizedAccessException();

            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId && u.IsActive, ct)
                ?? throw new UnauthorizedAccessException("Usuario no encontrado o inactivo.");

            var token = jwtTokenGenerator.GenerateToken(user);

            return new AuthResult(user.Id, user.RestaurantId, user.FullName, user.Email, user.Role.ToString(), token);
        }
    }
}