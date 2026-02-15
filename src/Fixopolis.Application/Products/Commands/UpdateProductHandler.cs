using Fixopolis.Application.Abstractions;
using Fixopolis.Application.Products.Exceptions;
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

        var categoryExists = await db.Categories.AnyAsync(c => c.Id == req.CategoryId, ct);
        if (!categoryExists)
            throw new CategoryNotFoundException(req.CategoryId);

        var codeUsedByAnother = await db.Products.AnyAsync(p => p.Code == req.Code && p.Id != req.Id, ct);
        if (codeUsedByAnother)
            throw new ProductCodeAlreadyExistsException(req.Code);

        if (!string.IsNullOrWhiteSpace(req.ImageUrl))
        {
            await imageDeleter.DeleteImageAsync(product.ImageUrl, ct);
            product.ImageUrl = req.ImageUrl;
        }

        product.Name = req.Name;
        product.Code = req.Code;
        product.CategoryId = req.CategoryId;
        product.Description = req.Description;
        product.Price = req.Price;
        product.Stock = req.Stock;
        product.IsAvailable = req.IsAvailable;

        await db.SaveChangesAsync(ct);
        return true;
    }
}
