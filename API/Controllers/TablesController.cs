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
                var command = new UpdateTableCommand(id, request.Code, request.IsActive);
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

                // Se devuelve el código QR como un archivo PNG para que el cliente lo pueda descargar
                return File(qrCodeBytes, "image/png");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
        }
    }
}