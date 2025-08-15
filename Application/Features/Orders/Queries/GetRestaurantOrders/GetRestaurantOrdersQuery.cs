using Application.Features.Orders.Queries.GetOrderByCode;
using MediatR;
using System;
using System.Collections.Generic;

namespace Application.Features.Orders.Queries.GetRestaurantOrders
{
    public record GetRestaurantOrdersQuery(Guid RestaurantId) : IRequest<List<OrderDetailDto>>;
}
