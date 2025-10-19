namespace Fixopolis.Application.Products.Dtos;

public sealed class ProductDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "";
    public string Code { get; init; } = "";
    public string? CategoryName { get; init; } = "";
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public int Stock { get; init; }
    public bool IsAvailable { get; init; }
}

public sealed record UpdateProductRequest(
    string Name,
    string Code,
    string? Description,
    decimal Price,
    int Stock,
    bool IsAvailable,
    Guid? CategoryId,
    string? CategoryName
);




