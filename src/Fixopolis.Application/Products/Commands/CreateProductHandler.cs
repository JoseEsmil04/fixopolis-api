using Fixopolis.Application.Abstractions;
using Fixopolis.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixopolis.Application.Products.Commands;

public sealed class CreateProductHandler(IAppDbContext db)
    : IRequestHandler<CreateProductCommand, Guid>
{
    public async Task<Guid> Handle(CreateProductCommand req, CancellationToken ct)
    {
        var validCategoryIds = await db.Categories
            .Where(c => req.CategoryIds.Contains(c.Id))
            .Select(c => c.Id)
            .ToListAsync(ct);

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = req.Name,
            Code = req.Code,
            Description = req.Description,
            Price = req.Price,
            Stock = req.Stock,
            IsAvailable = req.IsAvailable,
            ProductCategories = validCategoryIds
                .Select(catId => new ProductCategory
                {
                    ProductId = Guid.Empty,
                    CategoryId = catId
                })
                .ToList()
        };

        foreach (var pc in product.ProductCategories)
            pc.ProductId = product.Id;

        db.Products.Add(product);
        await db.SaveChangesAsync(ct);
        return product.Id;
    }
}
