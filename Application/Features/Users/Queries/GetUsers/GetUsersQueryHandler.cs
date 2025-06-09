using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Users.Queries.GetUsers
{
    public class GetUsersQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService) : IRequestHandler<GetUsersQuery, List<UserDto>>
    {
        public async Task<List<UserDto>> Handle(GetUsersQuery request, CancellationToken ct)
        {
            var restaurantId = currentUserService.RestaurantId ?? throw new UnauthorizedAccessException();

            return await dbContext.Users
                .AsNoTracking()
                .Where(u => u.RestaurantId == restaurantId)
                .Select(u => new UserDto(u.Id, u.FullName, u.Email, u.Role.ToString(), u.IsActive))
                .ToListAsync(ct);
        }
    }
}