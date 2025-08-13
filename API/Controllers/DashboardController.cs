using Application.Features.Dashboard.Queries.GetDashboardSummary;
using Application.Features.Dashboard.Queries.GetTopDishesToday;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // Solo los admins pueden ver el dashboard
    public class DashboardController(ISender mediator) : ControllerBase
    {
        [HttpGet("summary")]
        public async Task<IActionResult> GetDashboardSummary()
        {
            var summary = await mediator.Send(new GetDashboardSummaryQuery());
            return Ok(summary);
        }

        [HttpGet("top-dishes-today")]
        public async Task<IActionResult> GetTopDishesToday()
        {
            var topDishes = await mediator.Send(new GetTopDishesTodayQuery());
            return Ok(topDishes);
        }
    }
}