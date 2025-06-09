using MediatR;

namespace Application.Features.Tables.Queries.GetTableQrCode
{
    public record GetTableQrCodeQuery(Guid TableId) : IRequest<byte[]>;
}