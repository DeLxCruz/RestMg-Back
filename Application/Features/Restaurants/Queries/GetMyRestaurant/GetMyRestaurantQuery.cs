using MediatR;

namespace Application.Features.Restaurants.Queries.GetMyRestaurant
{
    public record GetMyRestaurantQuery : IRequest<MyRestaurantDto>;
}