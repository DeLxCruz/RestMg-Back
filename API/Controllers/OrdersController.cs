using Application.Features.Orders.Commands.CreateOrder;
using Application.Features.Orders.Queries.GetOrderByCode;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Kitchen, Admin")] // Permitir acceso a usuarios autenticados
    public class OrdersController(ISender mediator) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderCommand command)
        {
            try
            {
                var result = await mediator.Send(command);
                // Se devuelve el código del pedido para que el cliente pueda consultarlo
                return CreatedAtAction(nameof(GetOrderByCode), new { code = result.OrderCode, restaurantId = command.RestaurantId }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> GetOrderByCode([FromQuery] Guid restaurantId, string code)
        {
            try
            {
                var query = new GetOrderByCodeQuery(restaurantId, code);
                var order = await mediator.Send(query);
                return Ok(order);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpGet("restaurant/{restaurantId}")]
        public async Task<IActionResult> GetRestaurantOrders(Guid restaurantId)
        {
            var query = new Application.Features.Orders.Queries.GetRestaurantOrders.GetRestaurantOrdersQuery(restaurantId);
            var orders = await mediator.Send(query);
            return Ok(orders);
        }
    }
}