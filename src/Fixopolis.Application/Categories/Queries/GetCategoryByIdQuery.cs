using Fixopolis.Application.Categories.Dtos;
using Fixopolis.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixopolis.Application.Categories.Queries;

public record GetCategoryByIdQuery(Guid Id) : IRequest<CategoryDto?>;

public class GetCategoryByIdHandler(IAppDbContext _db) : IRequestHandler<GetCategoryByIdQuery, CategoryDto?>
{
    public async Task<CategoryDto?> Handle(GetCategoryByIdQuery request, CancellationToken ct)
    {
        return await _db.Categories
            .AsNoTracking()
            .Where(c => c.Id == request.Id)
            .Select(c => new CategoryDto { Id = c.Id, Name = c.Name!, Slug = c.Slug ?? string.Empty })
            .FirstOrDefaultAsync(ct);
    }
}
