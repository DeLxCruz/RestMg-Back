using MediatR;

namespace Application.Features.Kitchen.Commands.StartOrder
{
    public record StartOrderCommand(Guid Id) : IRequest;
}