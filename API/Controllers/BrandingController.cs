using Application.Features.Branding.Commands.UploadLogo;
using Application.Features.Branding.Commands.UpdateRestaurantLogo;
using Application.Features.Branding.Queries.GetRestaurantLogo;
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

        [HttpPost("banner")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> UploadBanner(IFormFile file)
        {
            try
            {
                var command = new UploadLogoCommand(file);
                var bannerUrl = await mediator.Send(command);
                return Ok(new { url = bannerUrl });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("restaurant/{restaurantId}/logo")]
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateRestaurantLogo(Guid restaurantId, IFormFile file)
        {
            try
            {
                var command = new UpdateRestaurantLogoCommand(restaurantId, file);
                var logoUrl = await mediator.Send(command);
                return Ok(new { url = logoUrl, message = "Logo actualizado correctamente" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("restaurant/{restaurantId}/logo")]
        [AllowAnonymous] // Permitir acceso público para mostrar logos
        [ProducesResponseType(typeof(string), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetRestaurantLogo(Guid restaurantId)
        {
            var query = new GetRestaurantLogoQuery(restaurantId);
            var logoUrl = await mediator.Send(query);
            
            if (string.IsNullOrEmpty(logoUrl))
            {
                return NotFound(new { message = "El restaurante no tiene logo configurado." });
            }

            return Ok(new { url = logoUrl });
        }
    }
}