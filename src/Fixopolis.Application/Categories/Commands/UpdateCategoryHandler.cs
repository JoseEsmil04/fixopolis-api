using Fixopolis.Application.Abstractions;
using Fixopolis.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixopolis.Application.Categories.Commands;

public class UpdateCategoryHandler(IAppDbContext _db) : IRequestHandler<UpdateCategoryCommand, bool>
{
    public async Task<bool> Handle(UpdateCategoryCommand request, CancellationToken ct)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == request.Id, ct);

        if (category is null) return false;

        var nameTaken = await _db.Categories
            .AnyAsync(c => c.Id != request.Id && c.Name == request.Name, ct);
        if (nameTaken) throw new InvalidOperationException($"Ya existe una categoría con el nombre '{request.Name}'.");

        category.Name = request.Name;
        category.Slug = SlugHelper.GenerateSlug(request.Name);
        await _db.SaveChangesAsync(ct);

        return true;
    }
}