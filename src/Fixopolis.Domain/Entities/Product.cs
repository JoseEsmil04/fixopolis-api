using Fixopolis.Domain.Common;

namespace Fixopolis.Domain.Entities;

public class Product : BaseEntity
{
    public string? Name { get; set; }
    public string? Code { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; set; } = default!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsAvailable { get; set; }
    public string? ImageUrl { get; set; }
    // 1–N con OrderItem (un producto aparece en muchas líneas de pedido)
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
