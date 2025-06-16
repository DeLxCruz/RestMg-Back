using Application.Features.Reports.Queries.GetBestsellersReport;
using Application.Features.Reports.Queries.GetOrdersReport;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class ReportsController(ISender mediator) : ControllerBase
    {
        [HttpGet("orders")]
        public async Task<IActionResult> GetOrdersReport([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var query = new GetOrdersReportQuery(from, to);
            var report = await mediator.Send(query);
            return Ok(report);
        }

        [HttpGet("bestsellers")]
        public async Task<IActionResult> GetBestsellersReport([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var query = new GetBestsellersReportQuery(from, to);
            var report = await mediator.Send(query);
            return Ok(report);
        }
    }
}