using MediatR;

namespace Application.Features.Dashboard.Queries.GetTopDishesToday
{
    public record GetTopDishesTodayQuery : IRequest<List<TopDishDto>>;
}
