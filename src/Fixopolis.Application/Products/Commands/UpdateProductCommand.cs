using MediatR;

namespace Fixopolis.Application.Products.Commands;

public sealed record UpdateProductCommand(
    Guid Id,
    string Name,
    string Code,
    Guid CategoryId,
    string? Description,
    decimal Price,
    int Stock,
    bool IsAvailable,
    string? ImageUrl,
    string? CategoryName = null
) : IRequest<bool>;
