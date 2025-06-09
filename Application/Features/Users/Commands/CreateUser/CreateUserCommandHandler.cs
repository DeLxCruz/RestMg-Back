using Application.Common.Interfaces;
using Domain.Enums;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Commands.CreateUser
{
    public class CreateUserCommandHandler(
        IApplicationDbContext dbContext,
        IPasswordService passwordService,
        ICurrentUserService currentUserService) : IRequestHandler<CreateUserCommand, Guid>
    {
        public async Task<Guid> Handle(CreateUserCommand command, CancellationToken ct)
        {
            var restaurantId = currentUserService.RestaurantId ??
                throw new UnauthorizedAccessException("No se pudo identificar el restaurante del usuario.");

            if (await dbContext.Users.AnyAsync(u => u.Email == command.Email && u.RestaurantId == restaurantId, ct))
            {
                throw new Exception("El email ya está registrado en este restaurante.");
            }

            if (!Enum.TryParse<UserRole>(command.Role, true, out var userRole))
            {
                throw new Exception("Rol de usuario no válido.");
            }

            var user = new User
            {
                FullName = command.FullName,
                Email = command.Email,
                PasswordHash = passwordService.HashPassword(command.Password),
                Role = userRole,
                RestaurantId = restaurantId
            };

            await dbContext.Users.AddAsync(user, ct);
            await dbContext.SaveChangesAsync(ct);

            return user.Id;
        }
    }
}