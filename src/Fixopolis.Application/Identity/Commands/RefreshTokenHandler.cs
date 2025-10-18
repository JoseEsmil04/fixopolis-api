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

    public async Task<AuthResultDto> Handle(RefreshTokenCommand req, CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(
            u => u.RefreshToken == req.RefreshToken && u.IsActive, ct);

        if (user is null || user.RefreshTokenExpiresAt is null || user.RefreshTokenExpiresAt <= DateTime.UtcNow)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        var access = _tokens.GenerateAccessToken(user);
        var rt = _tokens.GenerateRefreshToken();
        var refresh = rt.Token;
        var exp = rt.ExpiresAt;


        user.RefreshToken = refresh;
        user.RefreshTokenExpiresAt = exp;
        await _db.SaveChangesAsync(ct);

        var dto = new UserDto(user.Id, user.Name!, user.Email!, user.Role, user.IsActive);
        return new AuthResultDto(access, refresh, exp, dto);
    }
}
