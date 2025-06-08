using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Auth.Commands.Login
{
    // Inyectamos las INTERFACES
    public class LoginCommandHandler(
        IApplicationDbContext dbContext,
        IJwtTokenGenerator jwtTokenGenerator,
        IPasswordService passwordService) : IRequestHandler<LoginCommand, AuthResult>
    {
        public async Task<AuthResult> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            // 1. Buscar al usuario por su email
            var user = await dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == command.Email, cancellationToken);

            // 2. Validar que el usuario existe y la contraseña es correcta
            if (user is null || !passwordService.VerifyPassword(command.Password, user.PasswordHash))
            {
                throw new Exception("Invalid email or password.");
            }

            if (!user.IsActive)
            {
                throw new Exception("User is inactive.");
            }

            // 3. Generar el token JWT
            var token = jwtTokenGenerator.GenerateToken(user);

            // 4. Devolver el resultado
            return new AuthResult(
                user.Id,
                user.RestaurantId,
                user.FullName,
                user.Email,
                user.Role.ToString(),
                token
            );
        }
    }
}