using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public record UpdateTableRequest(
        [Required][StringLength(10)] string Code,
        bool IsActive
    );
}