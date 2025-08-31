using MediatR;
using System;

namespace Application.Features.Orders.Commands.RejectOrder
{
    public record RejectOrderCommand(Guid OrderId, string? Reason) : IRequest;
}
