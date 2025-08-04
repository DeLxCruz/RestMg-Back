using API.DTOs;
using Application.Features.Tables.Commands.CreateTable;
using Application.Features.Tables.Commands.UpdateTable;
using Application.Features.Tables.Queries.GetTableQrCode;
using Application.Features.Tables.Queries.GetTables;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TablesController(ISender mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetTables()
        {
            var query = new GetTablesQuery();
            var tables = await mediator.Send(query);
            return Ok(tables);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTable([FromBody] CreateTableRequest request)
        {
            try
            {
                var command = new CreateTableCommand(request.Code);
                var tableId = await mediator.Send(command);
                return CreatedAtAction(null, new { id = tableId }, new { id = tableId });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTable(Guid id, [FromBody] UpdateTableRequest request)
        {
            try
            {
                var command = new UpdateTableCommand(id, request.Code, request.Status);
                await mediator.Send(command);
                return NoContent();
            }
            catch (Exception ex) when (ex is KeyNotFoundException or UnauthorizedAccessException or ArgumentException)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}/qr")]
        public async Task<IActionResult> GetTableQrCode(Guid id)
        {
            try
            {
                var query = new GetTableQrCodeQuery(id);
                var qrCodeBytes = await mediator.Send(query);

                return File(qrCodeBytes, "image/png");
            }
            catch (KeyNotFoundException ex)
            {
                // 404 - La mesa con ese ID no existe.
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                // 403 - No tienes permiso para ver esta mesa.
                return Forbid(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                // 400 - La petición es mala porque falta una pre-condición (el subdominio).
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}