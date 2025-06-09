using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public record CreateTableRequest([Required][StringLength(10)] string Code);
}