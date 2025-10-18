using Fixopolis.Domain.Common;
using Fixopolis.Domain.Utils;

namespace Fixopolis.Domain.Entities;

public class User : BaseEntity
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiresAt { get; set; }
    public List<Order>? Orders { get; set; }
}
