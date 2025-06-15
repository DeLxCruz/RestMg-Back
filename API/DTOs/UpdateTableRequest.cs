using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace API.DTOs
{
    public record UpdateTableRequest(
        [Required][StringLength(10)] string Code,
        TableStatus Status
    );
}