using Fixopolis.Application.Abstractions;
using Fixopolis.Application.Products.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixopolis.Application.Products.Queries;

public sealed record GetProductsQuery(GetProductsDto Dto) : IRequest<PaginatedResponse<ProductDto>>;

public sealed class GetProductsHandler(IAppDbContext db)
    : IRequestHandler<GetProductsQuery, PaginatedResponse<ProductDto>>
{
    public async Task<PaginatedResponse<ProductDto>> Handle(GetProductsQuery request, CancellationToken ct)
    {
        var query = db.Products
            .AsNoTracking()
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name!,
                Code = p.Code!,
                CategoryId = p.CategoryId,
                CategoryName = p.Category!.Name!,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                IsAvailable = p.IsAvailable,
                ImageUrl = p.ImageUrl
            });

        // Apply text search filter
        if (!string.IsNullOrWhiteSpace(request.Dto.Query))
        {
            query = query.Where(p => 
                p.Name.ToLower().Contains(request.Dto.Query!.ToLower()) ||
                p.Code.ToLower().Contains(request.Dto.Query!.ToLower()) ||
                (p.Description != null && p.Description.ToLower().Contains(request.Dto.Query!.ToLower())));
        }

        // Apply categories filter
        if (request.Dto.Categories != null && request.Dto.Categories.Any())
        {
            query = query.Where(p => 
                p.CategoryName != null && request.Dto.Categories!.Contains(p.CategoryName));
        }

        var totalCount = await query.CountAsync(ct);
        
        var data = await query
            .Skip(request.Dto.Offset)
            .Take(request.Dto.Limit)
            .ToListAsync(ct);

        var totalPages = (int)Math.Ceiling((double)totalCount / request.Dto.Limit);

        return new PaginatedResponse<ProductDto>
        {
            Data = data,
            Count = totalCount,
            TotalPages = totalPages
        };
    }
}
