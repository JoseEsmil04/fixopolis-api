using Fixopolis.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixopolis.Application.Products.Commands;

public sealed class UpdateProductHandler(IAppDbContext db, IProductImageDeleter imageDeleter)
    : IRequestHandler<UpdateProductCommand, bool>
{
    public async Task<bool> Handle(UpdateProductCommand req, CancellationToken ct)
    {
        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == req.Id, ct);
        if (product is null) return false;

        var category = await db.Categories.FirstOrDefaultAsync(c => c.Name == req.CategoryName!.Trim(), ct);
        if (category is null) throw new InvalidOperationException("La categoría indicada no existe.");

        var codeUsedByAnother = await db.Products.AnyAsync(p => p.Code == req.Code && p.Id != req.Id, ct);
        if (codeUsedByAnother) throw new InvalidOperationException("El código de producto ya está en uso.");

        if (!string.IsNullOrWhiteSpace(req.ImageUrl))
        {
            // Delete the previous image; ignore errors if the file does not exist.
            await imageDeleter.DeleteImageAsync(product.ImageUrl, ct);
            product.ImageUrl = req.ImageUrl;
        }

        product.Name = req.Name;
        product.Code = req.Code;
        product.CategoryId = category.Id;
        product.Description = req.Description;
        product.Price = req.Price;
        product.Stock = req.Stock;
        product.IsAvailable = req.IsAvailable;

        await db.SaveChangesAsync(ct);
        return true;
    }
}
