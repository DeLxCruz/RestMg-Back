using API.DTOs;
using Application.Features.Restaurants.Commands.Onboard;
using Application.Features.Restaurants.Commands.UpdateMyRestaurant;
using Application.Features.Restaurants.Queries.GetMyRestaurant;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RestaurantsController(ISender mediator) : ControllerBase
    {
        [HttpPost("onboard")]
        [ProducesResponseType(typeof(OnboardRestaurantResult), 201)] // 201 Created
        [ProducesResponseType(typeof(ProblemDetails), 400)] // 400 Bad Request
        public async Task<IActionResult> Onboard([FromBody] OnboardRestaurantRequest request)
        {
            try
            {
                var command = new OnboardRestaurantCommand(
                    request.RestaurantName,
                    request.AdminFullName,
                    request.AdminEmail,
                    request.AdminPassword
                );

                var result = await mediator.Send(command);

                // Se devuelve 201 Created con la información de las entidades creadas
                return CreatedAtAction(null, result);
            }
            catch (Exception ex)
            {
                // Si el handler lanza una excepción, se captura y se devuelve un 400 Bad Request
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyRestaurant()
        {
            var query = new GetMyRestaurantQuery();
            var restaurant = await mediator.Send(query);
            return Ok(restaurant);
        }

        [HttpPut("me")]
        [Authorize(Roles = "Admin")] // Solo el Admin puede editar los datos
        public async Task<IActionResult> UpdateMyRestaurant([FromBody] UpdateRestaurantRequest request)
        {
            var command = new UpdateMyRestaurantCommand(request.Name, request.BrandingColor, request.LogoUrl, request.ClientUrl);
            await mediator.Send(command);
            return NoContent();
        }
    }
}