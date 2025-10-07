using MediatR;

namespace Fixopolis.Application.Categories.Commands;

public sealed record DeleteCategoryCommand(Guid Id) : IRequest<bool>;