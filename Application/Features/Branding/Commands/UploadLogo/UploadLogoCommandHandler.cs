using Application.Common.Interfaces;
using MediatR;

namespace Application.Features.Branding.Commands.UploadLogo
{
    public class UploadLogoCommandHandler(IFileStorageService fileStorageService)
        : IRequestHandler<UploadLogoCommand, string>
    {
        public async Task<string> Handle(UploadLogoCommand command, CancellationToken ct)
        {
            if (command.File == null || command.File.Length == 0)
            {
                throw new ArgumentException("El archivo no puede estar vacío.");
            }

            await using var stream = command.File.OpenReadStream();
            var fileUrl = await fileStorageService.SaveFileAsync(stream, command.File.FileName);

            return fileUrl;
        }
    }
}