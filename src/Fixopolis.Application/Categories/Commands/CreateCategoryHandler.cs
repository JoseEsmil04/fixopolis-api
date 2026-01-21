using Fixopolis.Application.Abstractions;
using Fixopolis.Application.Common;
using Fixopolis.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixopolis.Application.Categories.Commands;

public sealed class CreateCategoryHandler(IAppDbContext _db) : IRequestHandler<CreateCategoryCommand, Guid>
{
    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken ct)
    {
        var exists = await _db.Categories.AnyAsync(c => c.Name == request.Name, ct);
        if (exists) throw new InvalidOperationException($"La categoría '{request.Name}' ya existe.");

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Slug = SlugHelper.GenerateSlug(request.Name)
        };

        _db.Categories.Add(category);
        await _db.SaveChangesAsync(ct);

        return category.Id;
    }
}
