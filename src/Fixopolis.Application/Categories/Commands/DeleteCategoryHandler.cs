using Fixopolis.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixopolis.Application.Categories.Commands;

public sealed class DeleteCategoryHandler(IAppDbContext _db) : IRequestHandler<DeleteCategoryCommand, bool>
{
    public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken ct)
    {
        var category = await _db.Categories.FirstOrDefaultAsync(c => c.Id == request.Id, ct);

        if (category is null) return false;

        _db.Categories.Remove(category);
        await _db.SaveChangesAsync(ct);

        return true;
    }
}
