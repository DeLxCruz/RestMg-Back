using MediatR;

namespace Application.Features.Kitchen.Commands.MarkOrderReady
{
    public record MarkOrderReadyCommand(Guid Id) : IRequest;
}