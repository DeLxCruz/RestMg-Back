using Application.Common.Interfaces;
using Application.Features.Categories.Commands.CreateCategory;
using Domain.Models;
using MediatR;

namespace Application.Features.Categories.Commands.CreateCategory;
public class CreateCategoryCommandHandler(IApplicationDbContext dbContext, ICurrentUserService currentUser)
    : IRequestHandler<CreateCategoryCommand, Guid>
{
    public async Task<Guid> Handle(CreateCategoryCommand command, CancellationToken ct)
    {
        var restaurantId = currentUser.RestaurantId ?? throw new UnauthorizedAccessException();
        var category = new Category
        {
            Name = command.Name,
            DisplayOrder = command.DisplayOrder,
            RestaurantId = restaurantId
        };
        await dbContext.Categories.AddAsync(category, ct);
        await dbContext.SaveChangesAsync(ct);
        return category.Id;
    }
}
