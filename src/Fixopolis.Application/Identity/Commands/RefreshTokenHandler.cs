using Fixopolis.Application.Abstractions;
using Fixopolis.Application.Identity.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixopolis.Application.Identity.Commands.RefreshToken;

public sealed class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, AuthResultDto>
{
    private readonly IAppDbContext _db;
    private readonly ITokenService _tokens;

    public RefreshTokenHandler(IAppDbContext db, ITokenService tokens)
    {
        _db = db; _tokens = tokens;
    }

    public static string TokenHash(string token)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = System.Text.Encoding.UTF8.GetBytes(token);
        return Convert.ToHexString(sha.ComputeHash(bytes));
    }

    public async Task<AuthResultDto> Handle(RefreshTokenCommand req, CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;
        var tokenHash = TokenHash(req.RefreshToken);

        var user = await _db.Users.FirstOrDefaultAsync(u =>
            u.IsActive &&
            u.RefreshToken == tokenHash &&
            u.RefreshTokenExpiresAt != null &&
            u.RefreshTokenExpiresAt > now, ct);

        if (user is null)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        var access = _tokens.GenerateAccessToken(user);
        var rt = _tokens.GenerateRefreshToken();
        var newRefresh = rt.Token;
        var exp = rt.ExpiresAt;

        user.RefreshToken = TokenHash(newRefresh);
        user.RefreshTokenExpiresAt = exp;
        await _db.SaveChangesAsync(ct);

        var dto = new UserDto(user.Id, user.Name!, user.Email!, user.Role, user.IsActive);
        return new AuthResultDto(access, newRefresh, exp, dto);
    }

}
