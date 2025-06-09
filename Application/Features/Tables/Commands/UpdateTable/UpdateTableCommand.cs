using MediatR;

namespace Application.Features.Tables.Commands.UpdateTable
{
    public record UpdateTableCommand(
        Guid Id,
        string Code,
        bool IsActive
    ) : IRequest;
}