using Application.Features.Menu.Queries.GetFullMenu;
using Application.Features.Menu.Queries.GetMenuItemDetail;
using MediatR;
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
                // Si el handler lanza la excepción, devolvemos un 404 Not Found
                return NotFound(new { message = ex.Message });
            }
        }
    }
}