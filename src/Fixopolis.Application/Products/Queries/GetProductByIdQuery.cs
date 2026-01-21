using Fixopolis.Application.Abstractions;
using Fixopolis.Application.Products.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixopolis.Application.Products.Queries;

public sealed record GetProductByIdQuery(Guid Id) : IRequest<ProductDto?>;

public sealed class GetProductByIdHandler(IAppDbContext db)
    : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken ct)
    {
        try
        {
            var product = await db.Products
                  .AsNoTracking()
                  .Where(p => p.Id == request.Id)
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
                  .FirstOrDefaultAsync(ct);

            return product;
        }
        catch (Exception)
        {
            return null;
        }
    }
}
