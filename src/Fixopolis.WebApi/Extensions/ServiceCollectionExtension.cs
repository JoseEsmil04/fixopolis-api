// src/Fixopolis.WebApi/Extensions/ServiceCollectionExtension.cs
using Fixopolis.Application.Abstractions;
using Fixopolis.Infrastructure.Auth;
using Fixopolis.Infrastructure.Security;
using Fixopolis.WebApi.Infrastructure.Images;

namespace Fixopolis.WebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityInfrastructure(this IServiceCollection services, IConfiguration cfg)
    {
        var jwtSection = cfg.GetSection("Jwt");
        services.Configure<JwtOptions>(jwtSection);

        var secret = jwtSection["Secret"] ?? jwtSection["Key"];
        if (string.IsNullOrWhiteSpace(secret) || secret.Length < 32)
            throw new InvalidOperationException("Jwt:Secret/Key missing or too short (≥ 32 chars).");

        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();

        services.AddAuthorization();

        return services;
    }
}
