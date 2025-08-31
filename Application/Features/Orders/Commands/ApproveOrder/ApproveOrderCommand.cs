using MediatR;
using System;

namespace Application.Features.Orders.Commands.ApproveOrder
{
    public record ApproveOrderCommand(Guid OrderId) : IRequest;
}
