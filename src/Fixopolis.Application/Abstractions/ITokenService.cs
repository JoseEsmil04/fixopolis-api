using Fixopolis.Domain.Entities;

namespace Fixopolis.Application.Abstractions;

public interface ITokenService
{
    string GenerateAccessToken(User user);
    (string Token, DateTime ExpiresAt) GenerateRefreshToken();
}
