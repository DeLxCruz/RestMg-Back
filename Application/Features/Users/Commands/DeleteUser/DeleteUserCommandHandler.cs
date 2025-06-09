using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Users.Commands.DeleteUser
{
    public class DeleteUserCommandHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService) : IRequestHandler<DeleteUserCommand>
    {
        public async Task Handle(DeleteUserCommand command, CancellationToken ct)
        {
            var currentUserId = currentUserService.UserId ?? throw new UnauthorizedAccessException();
            var restaurantId = currentUserService.RestaurantId ?? throw new UnauthorizedAccessException();

            if (command.Id == currentUserId)
            {
                throw new Exception("No puedes desactivarte a ti mismo.");
            }

            var user = await dbContext.Users.FindAsync(command.Id) ?? throw new Exception("Usuario no encontrado.");

            if (user.RestaurantId != restaurantId)
            {
                throw new UnauthorizedAccessException("No tienes permiso para eliminar este usuario.");
            }

            user.IsActive = false;
            await dbContext.SaveChangesAsync(ct);
        }
    }
}