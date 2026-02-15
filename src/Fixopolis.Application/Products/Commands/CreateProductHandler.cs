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
        var category = await db.Categories
            .FirstOrDefaultAsync(c => c.Name!.ToLower() == req.CategoryName.Trim().ToLower(), ct);

        if (category is null)
            throw new InvalidOperationException($"La categoría '{req.CategoryName}' no existe.");

        var codeInUse = await db.Products.AnyAsync(p => p.Code == req.Code, ct);
        if (codeInUse)
            throw new InvalidOperationException("El código de producto ya está en uso.");

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = req.Name,
            Code = req.Code,
            Description = req.Description,
            Price = req.Price,
            Stock = req.Stock,
            IsAvailable = req.IsAvailable,
            CategoryId = category.Id,
            ImageUrl = req.ImageUrl
        };

        db.Products.Add(product);
        await db.SaveChangesAsync(ct);
        return product.Id;
    }
}

