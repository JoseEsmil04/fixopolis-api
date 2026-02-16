namespace Fixopolis.WebApi.Seed.Dtos;

public sealed class CreateProductForm
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsAvailable { get; set; } = true;
    public IFormFile? ImageFile { get; set; }
    public string? ImageUrl { get; set; }
}

public sealed class UpdateProductForm
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsAvailable { get; set; } = true;
    public IFormFile? ImageFile { get; set; }
    public string? ImageUrl { get; set; }
}
