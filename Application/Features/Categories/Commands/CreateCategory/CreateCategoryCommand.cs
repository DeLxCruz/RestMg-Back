using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace Application.Features.Categories.Commands.CreateCategory
{
    public record CreateCategoryCommand(string Name, int DisplayOrder) : IRequest<Guid>;
}