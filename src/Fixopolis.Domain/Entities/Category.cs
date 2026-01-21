using Fixopolis.Domain.Common;

namespace Fixopolis.Domain.Entities;

public class Category : BaseEntity
{
    public string? Name { get; set; }
    public string? Slug { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
}