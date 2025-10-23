// public sealed class UserSeedDto
// {
//     public string Name { get; set; } = default!;
//     public string Email { get; set; } = default!;
//     public string? Password { get; set; }
//     public string? PasswordHash { get; set; }
//     public int Role { get; set; }
//     public bool IsActive { get; set; }
// }

public sealed class ProductSeedDto
{
    public string Name { get; set; } = default!;
    public string Code { get; set; } = default!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsAvailable { get; set; }
    public string? Category { get; set; }
    public string? ImageUrl { get; set; }
}