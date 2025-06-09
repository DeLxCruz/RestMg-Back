namespace API.DTOs
{
    public record UpdateUserRequest(
        string FullName,
        string Email,
        string Role,
        bool IsActive
    );
}