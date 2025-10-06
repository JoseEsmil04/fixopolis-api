using MediatR;

namespace Fixopolis.Application.Products.Commands;

public sealed record UpdateProductCommand(
    Guid Id,
    string Name,
    string Code,
    string? Description,
    decimal Price,
    int Stock,
    bool IsAvailable,
    List<Guid> CategoryIds
) : IRequest<bool>;
