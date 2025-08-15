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

            // Validar que el restaurante tenga un slug configurado
            if (string.IsNullOrWhiteSpace(table.Restaurant.Subdomain))
            {
                throw new InvalidOperationException("Este restaurante no tiene un slug/subdominio configurado. Por favor, configúrelo en los ajustes.");
            }

            // Determinar la URL base del cliente
            var clientBaseUrl = configuration["ClientAppSettings:ClientUrl"]
               ?? throw new InvalidOperationException("La URL base del cliente (ClientUrl) no está configurada.");

            var urlToEncode = $"{clientBaseUrl}/r/{table.Restaurant.Subdomain}/menu/{table.Id}";

            var qrCodeBytes = qrCodeGenerator.Generate(urlToEncode);

            return qrCodeBytes;
        }
    }
}