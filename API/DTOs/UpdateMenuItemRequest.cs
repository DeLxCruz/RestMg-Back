namespace API.DTOs
{
    public record UpdateMenuItemRequest(Guid CategoryId, string Name, string? Description, decimal Price, string? ImageUrl, bool IsAvailable);
}