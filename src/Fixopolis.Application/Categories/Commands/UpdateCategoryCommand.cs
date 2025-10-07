using MediatR;

namespace Fixopolis.Application.Categories.Commands;

public record UpdateCategoryCommand(Guid Id, string Name) : IRequest<bool>;