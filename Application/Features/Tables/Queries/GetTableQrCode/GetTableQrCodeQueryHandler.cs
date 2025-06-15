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
                .Include(t => t.Restaurant)
                .FirstOrDefaultAsync(t => t.Id == request.TableId && t.RestaurantId == restaurantId, ct)
                ?? throw new KeyNotFoundException("Mesa no encontrada o no pertenece a su restaurante.");

            // Determinar la URL base del cliente
            var clientUrlBase = table.Restaurant.ClientUrl;

            if (string.IsNullOrWhiteSpace(clientUrlBase))
            {
                clientUrlBase = configuration["ClientAppSettings:ClientUrl"]
                    ?? throw new InvalidOperationException("No se ha configurado una URL de cliente por defecto.");
            }

            var urlToEncode = $"{clientUrlBase}/menu/{table.Code}";

            var qrCodeBytes = qrCodeGenerator.Generate(urlToEncode);

            return qrCodeBytes;
        }
    }
}