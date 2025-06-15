using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.MenuItems.Commands.DeleteMenuItem
{
    public class DeleteMenuItemCommandHandler(IApplicationDbContext db, ICurrentUserService user)
        : IRequestHandler<DeleteMenuItemCommand>
    {
        public async Task Handle(DeleteMenuItemCommand command, CancellationToken ct)
        {
            var restaurantId = user.RestaurantId ?? throw new UnauthorizedAccessException();
            var item = await db.MenuItems.FindAsync(command.Id) ?? throw new KeyNotFoundException("Plato no encontrado.");
            if (item.RestaurantId != restaurantId) throw new UnauthorizedAccessException();

            item.IsActive = false;
            await db.SaveChangesAsync(ct);
        }
    }
}