using MediatR;

namespace Fixopolis.Application.Products.Commands;

public sealed record CreateProductCommand(
    string Name,
    string Code,
    string? Description,
    Guid CategoryId,
    decimal Price,
    int Stock,
    bool IsAvailable,
    string? ImageUrl,
    string? CategoryName = null
) : IRequest<Guid>;
