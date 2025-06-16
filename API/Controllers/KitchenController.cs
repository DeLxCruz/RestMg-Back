using Application.Features.Kitchen.Commands.MarkOrderReady;
using Application.Features.Kitchen.Commands.StartOrder;
using Application.Features.Kitchen.ConfirmOrderPayment;
using Application.Features.Kitchen.Queries.GetKitchenOrders;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Kitchen, Admin")]
    public class KitchenController(IMediator mediator) : ControllerBase
    {
        [HttpGet("orders")]
        public async Task<IActionResult> GetOrders([FromQuery] string? status)
        {
            var query = new GetKitchenOrdersQuery(status);
            var orders = await mediator.Send(query);
            return Ok(orders);
        }

        [HttpPut("orders/{id}/start")]
        public async Task<IActionResult> StartOrder(Guid id)
        {
            try
            {
                await mediator.Send(new StartOrderCommand(id));
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("orders/{id}/ready")]
        public async Task<IActionResult> MarkOrderReady(Guid id)
        {
            try
            {
                await mediator.Send(new MarkOrderReadyCommand(id));
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("orders/{id}/confirm-payment")]
        public async Task<IActionResult> ConfirmPayment(Guid id)
        {
            try
            {
                await mediator.Send(new ConfirmOrderPaymentCommand(id));
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}