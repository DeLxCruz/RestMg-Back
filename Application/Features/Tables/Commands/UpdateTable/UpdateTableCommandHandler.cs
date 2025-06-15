using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Tables.Commands.UpdateTable
{
    public class UpdateTableCommandHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService) : IRequestHandler<UpdateTableCommand>
    {
        public async Task Handle(UpdateTableCommand command, CancellationToken ct)
        {
            var restaurantId = currentUserService.RestaurantId
                ?? throw new UnauthorizedAccessException("Restaurante no identificado.");

            var table = await dbContext.Tables.FindAsync(new object[] { command.Id }, ct)
                ?? throw new KeyNotFoundException("Mesa no encontrada.");

            if (table.RestaurantId != restaurantId)
            {
                throw new UnauthorizedAccessException("No tiene permiso para editar esta mesa.");
            }

            // Validar si el nuevo código ya está en uso por OTRA mesa
            var codeExists = await dbContext.Tables.AnyAsync(t =>
                t.RestaurantId == restaurantId &&
                t.Code == command.Code &&
                t.Id != command.Id, // Excluir la mesa actual de la búsqueda
                ct);

            if (codeExists)
            {
                throw new ArgumentException($"La mesa con el código '{command.Code}' ya existe.");
            }

            table.Code = command.Code;
            table.Status = command.Status;

            await dbContext.SaveChangesAsync(ct);
        }
    }
}