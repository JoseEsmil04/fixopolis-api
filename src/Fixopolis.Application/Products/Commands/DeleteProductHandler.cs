using Fixopolis.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixopolis.Application.Products.Commands;

public sealed class DeleteProductHandler(IAppDbContext db, IProductImageDeleter imageDeleter)
    : IRequestHandler<DeleteProductCommand, bool>
{
    public async Task<bool> Handle(DeleteProductCommand req, CancellationToken ct)
    {
        var product = await db.Products.FirstOrDefaultAsync(p => p.Id == req.Id, ct);
        if (product is null) return false;

        if (!string.IsNullOrWhiteSpace(product.ImageUrl))
        {
            await imageDeleter.DeleteImageAsync(product.ImageUrl, ct);
        }

        db.Products.Remove(product);
        await db.SaveChangesAsync(ct);
        return true;
    }
}

