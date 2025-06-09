using MediatR;

namespace Application.Features.Tables.Commands.CreateTable
{
    public record CreateTableCommand(string Code) : IRequest<Guid>;
}