using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public record CreateUserRequest(
        [Required] string FullName,
        [Required][EmailAddress] string Email,
        [Required] string Password,
        [Required] string Role
    );
}