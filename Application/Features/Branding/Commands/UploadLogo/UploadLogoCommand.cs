using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Features.Branding.Commands.UploadLogo
{
    public record UploadLogoCommand(IFormFile File) : IRequest<string>;
}