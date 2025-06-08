namespace Application.Features.Auth.Commands.Login
{
    public record AuthResult(
        Guid UserId,
        Guid RestaurantId,
        string FullName,
        string Email,
        string Role,
        string Token
    );
}