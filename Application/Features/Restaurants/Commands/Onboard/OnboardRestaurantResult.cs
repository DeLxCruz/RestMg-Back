namespace Application.Features.Restaurants.Commands.Onboard
{
    public record OnboardRestaurantResult(
        Guid RestaurantId,
        Guid AdminUserId
    );
}