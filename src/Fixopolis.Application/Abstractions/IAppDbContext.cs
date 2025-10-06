using Fixopolis.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fixopolis.Application.Abstractions;

public interface IAppDbContext
{
    DbSet<Product> Products { get; }
    DbSet<Category> Categories { get; }
    DbSet<ProductCategory> ProductCategories { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
