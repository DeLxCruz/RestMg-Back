using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public record OnboardRestaurantRequest(
        [Required]
        [StringLength(120, MinimumLength = 3)]
        string RestaurantName,

        [Required]
        [StringLength(80)]
        string AdminFullName,

        [Required]
        [EmailAddress]
        string AdminEmail,

        [Required]
        [StringLength(100, MinimumLength = 8)]
        string AdminPassword
    );
}