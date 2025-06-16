using MediatR;

namespace Application.Features.Kitchen.ConfirmOrderPayment
{
    public record ConfirmOrderPaymentCommand(Guid OrderId) : IRequest;
}