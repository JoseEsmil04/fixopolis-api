using System;
using Fixopolis.Domain.Common;

namespace Fixopolis.Domain.Entities;

public class Category : BaseEntity
{
    public string? Name { get; set; }
    // N–N con Product
    public List<ProductCategory>? ProductCategories { get; set; }
}