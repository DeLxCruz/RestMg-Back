using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Application.Features.Tables.Queries.GetTableQrCode
{
    public class GetTableQrCodeQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IQrCodeGenerator qrCodeGenerator,
        IConfiguration configuration)
        : IRequestHandler<GetTableQrCodeQuery, byte[]>
    {
        public async Task<byte[]> Handle(GetTableQrCodeQuery request, CancellationToken ct)
        {
            var restaurantId = currentUserService.RestaurantId
                ?? throw new UnauthorizedAccessException("Restaurante no identificado.");

            var table = await dbContext.Tables
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == request.TableId && t.RestaurantId == restaurantId, ct)
                ?? throw new KeyNotFoundException("Mesa no encontrada o no pertenece a su restaurante.");

            // Construir la URL que se codificará en el QR
            var clientUrl = configuration["ClientAppSettings:ClientUrl"]
                ?? throw new InvalidOperationException("La URL del cliente no está configurada.");

            var urlToEncode = $"{clientUrl}/menu/{table.Code}";

            // Generar el QR usando el servicio
            var qrCodeBytes = qrCodeGenerator.Generate(urlToEncode);

            return qrCodeBytes;
        }
    }
}