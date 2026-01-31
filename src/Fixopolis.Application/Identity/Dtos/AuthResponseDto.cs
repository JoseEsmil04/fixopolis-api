namespace Fixopolis.Application.Identity.Dtos;

public sealed record AuthResponseDto(
    string Token,
    UserDto User
);