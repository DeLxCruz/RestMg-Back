using Application.Common.Interfaces;
using Domain.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Tables.Commands.CreateTable
{
    public class CreateTableCommandHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService) : IRequestHandler<CreateTableCommand, Guid>
    {
        public async Task<Guid> Handle(CreateTableCommand command, CancellationToken ct)
        {
            var restaurantId = currentUserService.RestaurantId
                ?? throw new UnauthorizedAccessException("Restaurante no identificado.");

            var codeExists = await dbContext.Tables
                .AnyAsync(t => t.RestaurantId == restaurantId && t.Code == command.Code, ct);

            if (codeExists)
            {
                throw new ArgumentException($"La mesa con el código '{command.Code}' ya existe.");
            }

            var table = new Table
            {
                Code = command.Code,
                RestaurantId = restaurantId
            };

            await dbContext.Tables.AddAsync(table, ct);
            await dbContext.SaveChangesAsync(ct);

            return table.Id;
        }
    }
}