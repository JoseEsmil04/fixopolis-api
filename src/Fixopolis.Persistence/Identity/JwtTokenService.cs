// src/Fixopolis.Infrastructure/Auth/JwtTokenService.cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Fixopolis.Application.Abstractions;
using Fixopolis.Domain.Entities; // User
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Fixopolis.Infrastructure.Auth;

public class JwtTokenService : ITokenService
{
    private readonly JwtOptions _opt;
    private readonly SymmetricSecurityKey _key;
    public JwtTokenService(IOptions<JwtOptions> opt)
    {
        _opt = opt.Value;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.Secret));
    }

    public string GenerateAccessToken(User user)
    {
        var claims = new List<Claim>
    {
        new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new(JwtRegisteredClaimNames.UniqueName, user.Name ?? user.Email ?? user.Id.ToString()),
        new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
        new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

        // 👇 Convertimos el enum UserRole a string ("Admin", "Employee" o "Customer")
        new(ClaimTypes.Role, user.Role.ToString())
    };

        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _opt.Issuer,
            audience: _opt.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(_opt.AccessTokenMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public (string Token, DateTime ExpiresAt) GenerateRefreshToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        var token = Convert.ToBase64String(bytes);
        var exp = DateTime.UtcNow.AddDays(_opt.RefreshTokenDays);
        return (token, exp);
    }
}
