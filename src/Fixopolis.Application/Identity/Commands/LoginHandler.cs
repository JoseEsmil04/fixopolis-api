using Fixopolis.Application.Abstractions;
using Fixopolis.Application.Identity.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixopolis.Application.Identity.Commands.Login;

public sealed class LoginHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IAppDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokens;

    public LoginHandler(IAppDbContext db, IPasswordHasher hasher, ITokenService tokens)
    {
        _db = db; _hasher = hasher; _tokens = tokens;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand req, CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == req.Email && u.IsActive, ct);
        if (user is null || !_hasher.Verify(req.Password, user.PasswordHash!))
            throw new UnauthorizedAccessException("Invalid credentials.");

        var token = _tokens.GenerateAccessToken(user);

        var userDto = new UserDto(user.Id, user.Name!, user.Email!, user.Role, user.IsActive);
        return new AuthResponseDto(token, userDto);
    }
}
