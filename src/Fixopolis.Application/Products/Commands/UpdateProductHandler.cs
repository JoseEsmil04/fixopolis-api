using Fixopolis.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixopolis.Application.Products.Commands;

public sealed class UpdateProductHandler(IAppDbContext db)
    : IRequestHandler<UpdateProductCommand, bool>
{
    public async Task<bool> Handle(UpdateProductCommand req, CancellationToken ct)
    {
        var product = await db.Products
            .Include(p => p.ProductCategories)
            .FirstOrDefaultAsync(p => p.Id == req.Id, ct);

        if (product is null) return false;

        product.Name = req.Name;
        product.Code = req.Code;
        product.Description = req.Description;
        product.Price = req.Price;
        product.Stock = req.Stock;
        product.IsAvailable = req.IsAvailable;

        var currentIds = product.ProductCategories.Select(pc => pc.CategoryId).ToHashSet();
        var desiredIds = req.CategoryIds.ToHashSet();

        // quitar
        var toRemove = product.ProductCategories
            .Where(pc => !desiredIds.Contains(pc.CategoryId))
            .ToList();
        foreach (var pc in toRemove)
            product.ProductCategories.Remove(pc);

        // agregar
        var toAdd = desiredIds.Except(currentIds);
        foreach (var catId in toAdd)
            product.ProductCategories.Add(new Domain.Entities.ProductCategory
            {
                ProductId = product.Id,
                CategoryId = catId
            });

        await db.SaveChangesAsync(ct);
        return true;
    }
}
