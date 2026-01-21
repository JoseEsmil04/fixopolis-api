using Fixopolis.Application.Categories.Dtos;
using Fixopolis.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixopolis.Application.Categories.Queries;

public record GetCategoriesQuery() : IRequest<List<CategoryDto>>;

public sealed class GetCategoriesHandler(IAppDbContext _db) : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken ct)
    {
        return await _db.Categories
            .AsNoTracking()
            .Select(c => new CategoryDto { Id = c.Id, Name = c.Name!, Slug = c.Slug ?? string.Empty })
            .ToListAsync(ct);
    }
}
