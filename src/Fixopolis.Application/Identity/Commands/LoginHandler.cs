using Fixopolis.Application.Abstractions;
using Fixopolis.Application.Identity.Commands.RefreshToken;
using Fixopolis.Application.Identity.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixopolis.Application.Identity.Commands.Login;

public sealed class LoginHandler : IRequestHandler<LoginCommand, AuthResultDto>
{
    private readonly IAppDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokens;

    public LoginHandler(IAppDbContext db, IPasswordHasher hasher, ITokenService tokens)
    {
        _db = db; _hasher = hasher; _tokens = tokens;
    }

    public async Task<AuthResultDto> Handle(LoginCommand req, CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == req.Email && u.IsActive, ct);
        if (user is null || !_hasher.Verify(req.Password, user.PasswordHash!))
            throw new UnauthorizedAccessException("Invalid credentials.");

        var access = _tokens.GenerateAccessToken(user);
        string? refresh = null;
        DateTime? exp = null;

        var rt = _tokens.GenerateRefreshToken();
        refresh = rt.Token;
        exp = rt.ExpiresAt;


        if (refresh is not null)
        {
            user.RefreshToken = RefreshTokenHandler.TokenHash(refresh);
            user.RefreshTokenExpiresAt = exp;
            await _db.SaveChangesAsync(ct);
        }

        var dto = new UserDto(user.Id, user.Name!, user.Email!, user.Role, user.IsActive);
        return new AuthResultDto(access, refresh, exp, dto);
    }
}
