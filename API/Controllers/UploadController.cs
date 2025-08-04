using Application.Features.Branding.Commands.UploadLogo;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // Solo los admins pueden subir archivos
    public class UploadController(ISender mediator) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "No se ha proporcionado ningún archivo." });
            }

            try
            {
                var command = new UploadLogoCommand(file);
                var fileUrl = await mediator.Send(command);
                return Ok(new { url = fileUrl });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Capturar otros posibles errores del servicio de almacenamiento
                return StatusCode(500, new { message = $"Error interno al subir el archivo: {ex.Message}" });
            }
        }
    }
}