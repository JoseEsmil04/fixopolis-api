using Fixopolis.Domain.Common;

namespace Fixopolis.Domain.Entities;

public class Product : BaseEntity
{
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsAvailable { get; set; }

    // N–N con Category
    public List<ProductCategory>? ProductCategories { get; set; }

    // 1–N con OrderItem (un producto aparece en muchas líneas de pedido)
    public List<OrderItem>? OrderItems { get; set; }
}
