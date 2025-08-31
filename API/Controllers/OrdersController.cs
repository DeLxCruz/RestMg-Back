using Application.Common.Models;
using Application.Features.Orders.Commands.ApproveOrder;
using Application.Features.Orders.Commands.CreateOrder;
using Application.Features.Orders.Commands.DeliverOrder;
using Application.Features.Orders.Commands.RejectOrder;
using Application.Features.Orders.Queries.GetOrderByCode;
using Application.Features.Orders.Queries.GetRestaurantOrders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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

        [Authorize]
        [HttpGet("restaurant/{restaurantId}")]
        public async Task<IActionResult> GetRestaurantOrders(Guid restaurantId, [FromQuery] PaginationParams paginationParams)
        {
            var query = new GetRestaurantOrdersQuery(restaurantId, paginationParams.PageNumber, paginationParams.PageSize);
            var orders = await mediator.Send(query);
            return Ok(orders);
        }

        [HttpPost("{orderId}/approve")]
        [Authorize(Roles = "Awaiter, Admin")]
        public async Task<IActionResult> ApproveOrder(Guid orderId)
        {
            try
            {
                await mediator.Send(new ApproveOrderCommand(orderId));
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{orderId}/reject")]
        [Authorize(Roles = "Awaiter, Admin")]
        public async Task<IActionResult> RejectOrder(Guid orderId, [FromBody] RejectOrderRequest? request)
        {
            try
            {
                await mediator.Send(new RejectOrderCommand(orderId, request?.Reason));
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{orderId}/deliver")]
        [Authorize(Roles = "Awaiter, Admin")]
        public async Task<IActionResult> DeliverOrder(Guid orderId)
        {
            try
            {
                await mediator.Send(new DeliverOrderCommand(orderId));
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public record RejectOrderRequest(string? Reason);
}