using MediatR;

namespace Fixopolis.Application.Categories.Commands;

public sealed record CreateCategoryCommand(string Name) : IRequest<Guid>;
