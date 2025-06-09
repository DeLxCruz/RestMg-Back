using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Application.Features.Restaurants.Commands.Onboard
{
    public record OnboardRestaurantCommand(
        string RestaurantName,
        string AdminFullName,
        string AdminEmail,
        string AdminPassword
    ) : IRequest<OnboardRestaurantResult>;
}