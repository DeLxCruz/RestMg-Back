using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Tables.Queries.GetTables
{
    public class GetTablesQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService) : IRequestHandler<GetTablesQuery, List<TableDto>>
    {
        public async Task<List<TableDto>> Handle(GetTablesQuery request, CancellationToken ct)
        {
            var restaurantId = currentUserService.RestaurantId
                ?? throw new UnauthorizedAccessException("Restaurante no identificado.");

            var tables = await dbContext.Tables
                .AsNoTracking()
                .Where(t => t.RestaurantId == restaurantId)
                .Select(t => new TableDto(t.Id, t.Code, t.IsActive))
                .OrderBy(t => t.Code)
                .ToListAsync(ct);

            return tables;
        }
    }
}