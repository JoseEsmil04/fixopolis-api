using System.Text.Json;
using Fixopolis.Application.Abstractions;
using Fixopolis.Application.Common;
using Fixopolis.Application.Products.Dtos;
using Fixopolis.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fixopolis.Persistence;

public static class FixopolisSeed
{
    public static async Task SeedAsync(
        FixopolisDbContext context,
        string contentRootPath,
        IPasswordHasher hasher,
        CancellationToken ct = default)
    {
        var basePath = Path.Combine(contentRootPath, "Seed", "Data");

        async Task<List<T>> LoadJsonAsync<T>(string fileName)
        {
            var path = Path.Combine(basePath, fileName);
            if (!File.Exists(path))
            {
                Console.WriteLine($"⚠️ No existe el archivo: {path}");
                return new();
            }

            var json = await File.ReadAllTextAsync(path, ct);
            return JsonSerializer.Deserialize<List<T>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new();
        }

        await context.Database.MigrateAsync(ct);

        // 2 Users
        if (!await context.Users.AnyAsync(ct))
        {
            var usersRaw = await LoadJsonAsync<User>("users.json");
            if (usersRaw.Count == 0)
                Console.WriteLine("⚠️ users.json vacío o no encontrado.");

            var users = new List<User>();
            foreach (var u in usersRaw)
            {
                var plain = u.PasswordHash ?? "123456";

                users.Add(new User
                {
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role,
                    IsActive = u.IsActive,
                    PasswordHash = hasher.Hash(plain)
                });
            }

            if (users.Count > 0)
            {
                await context.Users.AddRangeAsync(users, ct);
                await context.SaveChangesAsync(ct);
                Console.WriteLine($"✅ Inserted {users.Count} Users (with hashed passwords)");
            }
        }

        // 3 Categories
        if (!await context.Categories.AnyAsync(ct))
        {
            var categoriesRaw = await LoadJsonAsync<Category>("categories.json");
            if (categoriesRaw.Count == 0)
                Console.WriteLine("⚠️ categories.json vacío o no encontrado.");

            var categories = new List<Category>();
            foreach (var cat in categoriesRaw)
            {
                var slug = SlugHelper.GenerateSlug(cat.Name ?? string.Empty);
                Console.WriteLine($"Generating slug for '{cat.Name}' -> '{slug}'");
                categories.Add(new Category
                {
                    Name = cat.Name,
                    Slug = slug
                });
            }

            // Debug: Print all categories with slugs
            foreach (var cat in categories)
            {
                Console.WriteLine($"DEBUG: Category '{cat.Name}' with Slug '{cat.Slug}'");
            }

            if (categories.Count > 0)
            {
                await context.Categories.AddRangeAsync(categories, ct);
                await context.SaveChangesAsync(ct);
                Console.WriteLine($"✅ Inserted {categories.Count} Categories with slugs");
            }
        }

        // 4 Products
        if (!await context.Products.AnyAsync(ct))
        {
            var productsRaw = await LoadJsonAsync<ProductSeedDto>("products.json");
            if (productsRaw.Count == 0)
                Console.WriteLine("⚠️ products.json vacío o no encontrado.");

            var categories = await context.Categories.AsNoTracking().ToListAsync(ct);
            var catByName = categories.ToDictionary(
                c => c.Name!.Trim(), c => c.Id, StringComparer.OrdinalIgnoreCase);

            var products = new List<Product>();
            foreach (var pr in productsRaw)
            {
                if (string.IsNullOrWhiteSpace(pr.Category) ||
                    !catByName.TryGetValue(pr.Category.Trim(), out var catId))
                {
                    Console.WriteLine($"⚠️ Categoría no encontrada para producto '{pr.Name}' -> '{pr.Category}'. Se omite.");
                    continue;
                }

                products.Add(new Product
                {
                    Name = pr.Name,
                    Code = pr.Code,
                    Description = pr.Description,
                    Price = pr.Price,
                    Stock = pr.Stock,
                    IsAvailable = pr.IsAvailable,
                    CategoryId = catId,
                    ImageUrl = pr.ImageUrl
                });
            }

            if (products.Count > 0)
            {
                await context.Products.AddRangeAsync(products, ct);
                await context.SaveChangesAsync(ct);
                Console.WriteLine($"✅ Inserted {products.Count} Products (with CategoryId)");
            }
        }

        // 5 Orders + Items
        if (!await context.Orders.AnyAsync(ct) && !await context.OrderItems.AnyAsync(ct))
        {
            var userIds = await context.Users.Select(u => u.Id).ToListAsync(ct);
            var products = await context.Products
                .Select(p => new { p.Id, p.Price })
                .ToListAsync(ct);

            if (userIds.Count == 0 || products.Count == 0)
            {
                Console.WriteLine("⚠️ No hay usuarios o productos suficientes para generar órdenes.");
                return;
            }

            var rnd = new Random();
            var orderCount = Math.Min(userIds.Count, 10);
            var orders = new List<Order>(orderCount);

            for (int i = 0; i < orderCount; i++)
            {
                orders.Add(new Order
                {
                    UserId = userIds[i % userIds.Count],
                    CreatedAt = DateTime.UtcNow.AddMinutes(-i * 7),
                    Total = 0m
                });
            }
            await context.Orders.AddRangeAsync(orders, ct);
            await context.SaveChangesAsync(ct);

            var items = new List<OrderItem>();
            foreach (var order in orders)
            {
                var itemsCount = rnd.Next(1, Math.Min(4, products.Count + 1));
                var productIndexes = Enumerable.Range(0, products.Count)
                                               .OrderBy(_ => rnd.Next())
                                               .Take(itemsCount);

                foreach (var idx in productIndexes)
                {
                    var p = products[idx];
                    var qty = rnd.Next(1, 4);
                    var line = p.Price * qty;

                    items.Add(new OrderItem
                    {
                        OrderId = order.Id,
                        ProductId = p.Id,
                        Quantity = qty,
                        UnitPrice = p.Price,
                        LineTotal = line
                    });

                    order.Total += line;
                }
            }

            await context.OrderItems.AddRangeAsync(items, ct);
            context.Orders.UpdateRange(orders);
            await context.SaveChangesAsync(ct);

            Console.WriteLine($"✅ Inserted {orders.Count} Orders");
            Console.WriteLine($"✅ Inserted {items.Count} OrderItems");
        }
    }
}
