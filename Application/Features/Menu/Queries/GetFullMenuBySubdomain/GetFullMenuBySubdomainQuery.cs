using Application.Features.Menu.Queries.GetFullMenu;
using MediatR;
using System.Collections.Generic;

namespace Application.Features.Menu.Queries.GetFullMenuBySubdomain
{
    public record GetFullMenuBySubdomainQuery(string Subdomain)
    : IRequest<MenuWithRestaurantDto?>;
}