using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Branding.Commands.UpdateRestaurantLogo
{
    public class UpdateRestaurantLogoCommandHandler(
        IFileStorageService fileStorageService,
        IApplicationDbContext context)
        : IRequestHandler<UpdateRestaurantLogoCommand, string>
    {
        public async Task<string> Handle(UpdateRestaurantLogoCommand command, CancellationToken ct)
        {
            // 1. Validar el archivo
            if (command.File == null || command.File.Length == 0)
            {
                throw new ArgumentException("El archivo no puede estar vacío.");
            }

            // 2. Buscar el restaurante
            var restaurant = await context.Restaurants
                .FindAsync([command.RestaurantId], ct);

            if (restaurant == null)
            {
                throw new ArgumentException("El restaurante no existe.");
            }

            // 3. Subir la imagen a Firebase Storage
            await using var stream = command.File.OpenReadStream();
            var logoUrl = await fileStorageService.SaveFileAsync(stream, command.File.FileName, restaurant.Id);

            // 4. Actualizar la URL del logo en la base de datos
            restaurant.LogoUrl = logoUrl;

            await context.SaveChangesAsync(ct);

            return logoUrl;
        }
    }
}
