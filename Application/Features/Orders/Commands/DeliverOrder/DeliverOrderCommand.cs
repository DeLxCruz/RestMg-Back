
using MediatR;

namespace Application.Features.Orders.Commands.DeliverOrder
{
    public record DeliverOrderCommand(Guid OrderId) : IRequest;
}
