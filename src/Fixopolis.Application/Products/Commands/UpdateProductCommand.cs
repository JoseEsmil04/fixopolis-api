using MediatR;

namespace Fixopolis.Application.Products.Commands;

public sealed record UpdateProductCommand(
    Guid Id,
    string Name,
    string Code,
    string? CategoryName,
    string? Description,
    decimal Price,
    int Stock,
    bool IsAvailable,
    string? ImageUrl
) : IRequest<bool>;
