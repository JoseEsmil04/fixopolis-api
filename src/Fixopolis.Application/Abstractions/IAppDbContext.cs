using Fixopolis.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fixopolis.Application.Abstractions;

public interface IAppDbContext
{
    DbSet<Product> Products { get; }
    DbSet<Category> Categories { get; }
    DbSet<User> Users { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
