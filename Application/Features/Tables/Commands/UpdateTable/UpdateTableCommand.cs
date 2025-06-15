using Domain.Enums;
using MediatR;

namespace Application.Features.Tables.Commands.UpdateTable
{
    public record UpdateTableCommand(
        Guid Id,
        string Code,
        TableStatus Status
    ) : IRequest;
}