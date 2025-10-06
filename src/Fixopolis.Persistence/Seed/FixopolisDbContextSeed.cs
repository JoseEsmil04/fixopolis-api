using System.Text.Json;
using Fixopolis.Domain.Entities;

namespace Fixopolis.Persistence;

public static class FixopolisDbContextSeed
{
    public static async Task SeedAsync(FixopolisDbContext context)
    {
        var basePath = Path.Combine(AppContext.BaseDirectory, "Seed", "Data");

        async Task<List<T>> LoadJsonAsync<T>(string fileName)
        {
            var path = Path.Combine(basePath, fileName);
            if (!File.Exists(path)) return new();
            var json = await File.ReadAllTextAsync(path);
            return JsonSerializer.Deserialize<List<T>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new();
        }

        if (!context.Users.Any())
        {
            var users = await LoadJsonAsync<User>("users.json");
            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync(); // Guarda primero los usuarios
        }

        if (!context.Categories.Any())
        {
            var categories = await LoadJsonAsync<Category>("categories.json");
            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }

        if (!context.Products.Any())
        {
            var products = await LoadJsonAsync<Product>("products.json");
            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }

        if (!context.Orders.Any())
        {
            var userIds = context.Users.Select(u => u.Id).ToList(); // Buscar usuarios existentes

            var orders = new List<Order>
            {
                new() { UserId = userIds[0], CreatedAt = DateTime.UtcNow, Total = 900 },
                new() { UserId = userIds[1], CreatedAt = DateTime.UtcNow.AddMinutes(-10), Total = 250 },
                new() { UserId = userIds[2], CreatedAt = DateTime.UtcNow.AddMinutes(-20), Total = 800 },
                new() { UserId = userIds[3], CreatedAt = DateTime.UtcNow.AddMinutes(-30), Total = 600 },
                new() { UserId = userIds[4], CreatedAt = DateTime.UtcNow.AddMinutes(-40), Total = 700 },
            };

            await context.Orders.AddRangeAsync(orders);
            await context.SaveChangesAsync();

            Console.WriteLine($"✅ Inserted {orders.Count} Orders");
        }

        if (!context.OrderItems.Any())
        {
            var orderIds = context.Orders.Select(o => o.Id).ToList();
            var productIds = context.Products.Select(p => p.Id).ToList();

            var items = new List<OrderItem>
            {
                new() { OrderId = orderIds[0], ProductId = productIds[0], Quantity = 2, UnitPrice = 450, LineTotal = 900 },
                new() { OrderId = orderIds[1], ProductId = productIds[1], Quantity = 1, UnitPrice = 250, LineTotal = 250 },
                new() { OrderId = orderIds[2], ProductId = productIds[2], Quantity = 1, UnitPrice = 800, LineTotal = 800 },
                new() { OrderId = orderIds[3], ProductId = productIds[3], Quantity = 1, UnitPrice = 600, LineTotal = 600 },
                new() { OrderId = orderIds[4], ProductId = productIds[4], Quantity = 1, UnitPrice = 700, LineTotal = 700 },
            };

            await context.OrderItems.AddRangeAsync(items);
            await context.SaveChangesAsync();

            Console.WriteLine($"✅ Inserted {items.Count} OrderItems");
        }

        if (!context.ProductCategories.Any())
        {
            var productIds = context.Products.Select(p => p.Id).ToList();
            var categoryIds = context.Categories.Select(c => c.Id).ToList();

            var productCategories = new List<ProductCategory>
            {
                new() { ProductId = productIds[0], CategoryId = categoryIds[0] },
                new() { ProductId = productIds[1], CategoryId = categoryIds[1] },
                new() { ProductId = productIds[2], CategoryId = categoryIds[2] },
                new() { ProductId = productIds[3], CategoryId = categoryIds[3] },
                new() { ProductId = productIds[4], CategoryId = categoryIds[4] },
            };

            await context.ProductCategories.AddRangeAsync(productCategories);
            await context.SaveChangesAsync();

            Console.WriteLine($"✅ Inserted {productCategories.Count} ProductCategories");
        }
    }
}
