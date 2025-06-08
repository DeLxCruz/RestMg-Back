using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public record LoginRequest(
        [Required] string Email,
        [Required] string Password
    );
}