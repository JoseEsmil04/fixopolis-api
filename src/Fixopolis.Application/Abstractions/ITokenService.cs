using Fixopolis.Domain.Entities;

namespace Fixopolis.Application.Abstractions;

public interface ITokenService
{
    string GenerateAccessToken(User user);
}
