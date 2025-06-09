using Application.Features.Branding.Commands.UploadLogo;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class BrandingController(ISender mediator) : ControllerBase
    {
        [HttpPost("logo")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UploadLogo(IFormFile file)
        {
            try
            {
                var command = new UploadLogoCommand(file);
                var logoUrl = await mediator.Send(command);
                return Ok(new { url = logoUrl });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}