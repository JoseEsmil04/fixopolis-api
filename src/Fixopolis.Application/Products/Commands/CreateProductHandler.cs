using Fixopolis.Application.Abstractions;
using Fixopolis.Application.Products.Exceptions;
using Fixopolis.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixopolis.Application.Products.Commands;

public sealed class CreateProductHandler(IAppDbContext db)
    : IRequestHandler<CreateProductCommand, Guid>
{
    public async Task<Guid> Handle(CreateProductCommand req, CancellationToken ct)
    {
        var categoryExists = await db.Categories.AnyAsync(c => c.Id == req.CategoryId, ct);
        if (!categoryExists)
            throw new CategoryNotFoundException(req.CategoryId);

        var codeInUse = await db.Products.AnyAsync(p => p.Code == req.Code, ct);
        if (codeInUse)
            throw new ProductCodeAlreadyExistsException(req.Code);

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = req.Name,
            Code = req.Code,
            Description = req.Description,
            Price = req.Price,
            Stock = req.Stock,
            IsAvailable = req.IsAvailable,
            CategoryId = req.CategoryId,
            ImageUrl = req.ImageUrl
        };

        db.Products.Add(product);
        await db.SaveChangesAsync(ct);
        return product.Id;
    }
}
