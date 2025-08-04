using Application.Features.Menu.Queries.GetFullMenu;
using Application.Features.Menu.Queries.GetFullMenuBySubdomain;
using Application.Features.Menu.Queries.GetMenuItemDetail;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenuController(ISender mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetMenu([FromQuery] Guid restaurantId)
        {
            var menu = await mediator.Send(new GetFullMenuQuery(restaurantId));
            return Ok(menu);
        }

        [HttpGet("{itemId}")]
        public async Task<IActionResult> GetMenuItemDetail(Guid itemId)
        {
            try
            {
                var query = new GetMenuItemDetailQuery(itemId);
                var menuItem = await mediator.Send(query);
                return Ok(menuItem);
            }
            catch (KeyNotFoundException ex)
            {
                // Si el handler lanza la excepción, se devuelve un 404 Not Found
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("by-subdomain/{subdomain}")]
        [AllowAnonymous] // Permitir acceso público para obtener el menú por subdominio
        public async Task<IActionResult> GetMenuBySubdomain(string subdomain)
        {
            var query = new GetFullMenuBySubdomainQuery(subdomain);
            var menu = await mediator.Send(query);

            // Si el handler devuelve null (restaurante no encontrado), se devuelve un 404.
            // Si devuelve una lista (incluso vacía), se devuelve 200 OK.
            return menu != null ? Ok(menu) : NotFound(new { message = "Restaurante no encontrado." });
        }
    }
}