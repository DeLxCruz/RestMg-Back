using MediatR;

namespace Application.Features.Kitchen.Commands.ConfirmOrderPayment
{
    public record ConfirmOrderPaymentCommand(Guid OrderId) : IRequest;
}