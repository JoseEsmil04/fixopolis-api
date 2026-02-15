namespace Fixopolis.Application.Products.Dtos;

public sealed class PaginatedResponse<T>
{
    public List<T> Data { get; init; } = new();
    public int Count { get; init; }
    public int TotalPages { get; init; }
}

public sealed class GetProductsDto
{
    public int Limit { get; init; } = 10;
    public int Offset { get; init; } = 0;
    public string? Query { get; init; }
    public List<string>? Categories { get; init; }
}

public sealed class ProductDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = "";
    public string Code { get; init; } = "";
    public Guid CategoryId { get; init; }
    public string? CategoryName { get; init; } = "";
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public int Stock { get; init; }
    public bool IsAvailable { get; init; }
    public string? ImageUrl { get; set; }
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
