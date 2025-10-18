using Fixopolis.Domain.Utils;

namespace Fixopolis.Application.Identity.Dtos;


public sealed record UserDto(Guid Id, string Name, string Email, UserRole Role, bool IsActive);
