using Fixopolis.Application.Abstractions;
using Fixopolis.Application.Products.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixopolis.Application.Products.Queries;

public sealed record GetProductsQuery() : IRequest<List<ProductDto>>;

public sealed class GetProductsHandler(IAppDbContext db)
    : IRequestHandler<GetProductsQuery, List<ProductDto>>
{
    public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken ct)
    {
        return await db.Products
            .AsNoTracking()
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name!,
                Code = p.Code!,
                CategoryName = p.Category!.Name!,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                IsAvailable = p.IsAvailable,
                ImageUrl = p.ImageUrl
            })
            .ToListAsync(ct);
    }
}
