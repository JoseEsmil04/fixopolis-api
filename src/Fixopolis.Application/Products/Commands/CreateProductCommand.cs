using MediatR;

namespace Fixopolis.Application.Products.Commands;

public sealed record CreateProductCommand(
    string Name,
    string Code,
    string? Description,
    string CategoryName,
    decimal Price,
    int Stock,
    bool IsAvailable,
    string? ImageUrl
) : IRequest<Guid>;
