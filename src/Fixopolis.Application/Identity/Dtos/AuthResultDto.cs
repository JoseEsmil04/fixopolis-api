namespace Fixopolis.Application.Identity.Dtos;

public sealed record AuthResultDto(
    string AccessToken,
    string? RefreshToken,
    DateTime? RefreshTokenExpiresAt,
    UserDto User
);
