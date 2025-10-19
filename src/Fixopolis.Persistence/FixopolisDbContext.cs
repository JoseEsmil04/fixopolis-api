using Fixopolis.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Fixopolis.Application.Abstractions;

namespace Fixopolis.Persistence;


public class FixopolisDbContext : DbContext, IAppDbContext
{
    public FixopolisDbContext(DbContextOptions<FixopolisDbContext> options) : base(options)
    { }

    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FixopolisDbContext).Assembly);
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(255);

            // 1:N User -> Orders
            entity.HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId);

        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("products");
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(p => p.Price)
                .HasPrecision(18, 2);

            entity.HasIndex(p => p.Code)
                .IsUnique()
                .HasDatabaseName("UX_products_code");

            entity.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(p => p.OrderItems)
                .WithOne(oi => oi.Product)
                .HasForeignKey(oi => oi.ProductId);
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.ToTable("categories");
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(c => c.Name)
                .IsUnique()
                .HasDatabaseName("UX_categories_name");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("orders");
            entity.HasKey(o => o.Id);

            entity.Property(o => o.Total)
                .HasPrecision(18, 2);

            // 1:N Order -> OrderItems
            entity.HasMany(o => o.Items)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("order_items");
            entity.HasKey(oi => oi.Id);

            entity.Property(oi => oi.UnitPrice)
                .HasPrecision(18, 2);

            entity.Property(oi => oi.LineTotal)
                .HasPrecision(18, 2);

            entity.HasOne(oi => oi.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(oi => oi.ProductId);
        });
    }
}
