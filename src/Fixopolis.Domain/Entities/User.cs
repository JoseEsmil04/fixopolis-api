using Fixopolis.Domain.Common;
using Fixopolis.Domain.Utils;

namespace Fixopolis.Domain.Entities;

public class User : BaseEntity
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }
    public List<Order>? Orders { get; set; }
}
