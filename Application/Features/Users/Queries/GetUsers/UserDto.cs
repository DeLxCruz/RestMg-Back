namespace Application.Features.Users.Queries.GetUsers
{
    public record UserDto(
        Guid Id,
        string FullName,
        string Email,
        string Role,
        bool IsActive
    );
}