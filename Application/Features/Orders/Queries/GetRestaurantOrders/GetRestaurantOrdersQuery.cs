using Application.Common.Models;
using Application.Features.Orders.Queries.GetOrderByCode;
using MediatR;
using System;
using System.Collections.Generic;

namespace Application.Features.Orders.Queries.GetRestaurantOrders
{
    public record GetRestaurantOrdersQuery(Guid RestaurantId, int PageNumber, int PageSize) : IRequest<PagedList<OrderDetailDto>>;
}
