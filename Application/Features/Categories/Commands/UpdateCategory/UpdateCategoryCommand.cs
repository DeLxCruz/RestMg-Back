using MediatR;

namespace Application.Features.Categories.Commands.UpdateCategory
{
    public record UpdateCategoryCommand(Guid Id, string Name, int DisplayOrder) : IRequest;
}