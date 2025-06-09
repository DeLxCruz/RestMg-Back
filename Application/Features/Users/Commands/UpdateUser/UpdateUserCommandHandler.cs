using Application.Common.Interfaces;
using Domain.Enums;
using MediatR;

namespace Application.Features.Users.Commands.UpdateUser
{
    public class UpdateUserCommandHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService) : IRequestHandler<UpdateUserCommand>
    {
        public async Task Handle(UpdateUserCommand command, CancellationToken ct)
        {
            var restaurantId = currentUserService.RestaurantId ?? throw new UnauthorizedAccessException();

            var user = await dbContext.Users.FindAsync(command.Id) ?? throw new Exception("Usuario no encontrado.");

            if (user.RestaurantId != restaurantId)
            {
                throw new UnauthorizedAccessException("No tienes permiso para editar este usuario.");
            }

            if (!Enum.TryParse<UserRole>(command.Role, true, out var userRole))
            {
                throw new Exception("Rol de usuario no válido.");
            }

            user.FullName = command.FullName;
            user.Email = command.Email;
            user.Role = userRole;
            user.IsActive = command.IsActive;

            await dbContext.SaveChangesAsync(ct);
        }
    }
}