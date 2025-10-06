using MediatR;

namespace Fixopolis.Application.Products.Commands;

public sealed record DeleteProductCommand(Guid Id) : IRequest<bool>;
