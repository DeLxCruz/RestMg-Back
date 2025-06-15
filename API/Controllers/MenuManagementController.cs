using API.DTOs;
using Application.Features.Categories.Commands.CreateCategory;
using Application.Features.Categories.Commands.DeleteCategory;
using Application.Features.Categories.Commands.UpdateCategory;
using Application.Features.MenuItems.Commands.CreateMenuItem;
using Application.Features.MenuItems.Commands.DeleteMenuItem;
using Application.Features.MenuItems.Commands.UpdateMenuItem;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/menu")]
    [Authorize(Roles = "Admin")]
    public class MenuManagementController(ISender mediator) : ControllerBase
    {
        [HttpPost("categories")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryCommand command)
        {
            var categoryId = await mediator.Send(command);
            return CreatedAtAction(null, new { id = categoryId });
        }

        [HttpPut("categories/{id}")]
        public async Task<IActionResult> UpdateCategory(Guid id, [FromBody] UpdateCategoryRequest request)
        {
            var command = new UpdateCategoryCommand(id, request.Name, request.DisplayOrder);
            await mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteCategory(Guid id)
        {
            await mediator.Send(new DeleteCategoryCommand(id));
            return NoContent();
        }

        [HttpPost("items")]
        public async Task<IActionResult> CreateMenuItem([FromBody] CreateMenuItemCommand command)
        {
            var itemId = await mediator.Send(command);
            return CreatedAtAction(null, new { id = itemId });
        }

        [HttpPut("items/{id}")]
        public async Task<IActionResult> UpdateMenuItem(Guid id, [FromBody] UpdateMenuItemRequest request)
        {
            var command = new UpdateMenuItemCommand(id, request.CategoryId, request.Name, request.Description, request.Price, request.ImageUrl, request.IsAvailable);
            await mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("items/{id}")]
        public async Task<IActionResult> DeleteMenuItem(Guid id)
        {
            await mediator.Send(new DeleteMenuItemCommand(id));
            return NoContent();
        }
    }
}