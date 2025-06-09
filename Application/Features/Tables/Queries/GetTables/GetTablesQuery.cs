using MediatR;

namespace Application.Features.Tables.Queries.GetTables
{
    public record GetTablesQuery : IRequest<List<TableDto>>;
}