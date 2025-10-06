using Fixopolis.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixopolis.Application.Products.Commands;

public sealed class DeleteProductHandler(IAppDbContext db)
    : IRequestHandler<DeleteProductCommand, bool>
{
    public async Task<bool> Handle(DeleteProductCommand req, CancellationToken ct)
    {
        var product = await db.Products
            .Include(p => p.ProductCategories)
            .FirstOrDefaultAsync(p => p.Id == req.Id, ct);

        if (product is null) return false;

        if (product.ProductCategories.Count > 0)
            db.ProductCategories.RemoveRange(product.ProductCategories);

        db.Products.Remove(product);
        await db.SaveChangesAsync(ct);
        return true;
    }
}

