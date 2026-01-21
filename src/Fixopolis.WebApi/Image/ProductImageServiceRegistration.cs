using Fixopolis.Application.Abstractions;

namespace Fixopolis.WebApi.Infrastructure.Images;

public static class ProductImageServicesRegistration
{
    public static IServiceCollection AddProductImageServices(this IServiceCollection services)
    {
        services.AddScoped<IProductImageStorage, SupabaseProductImageStorage>();
        services.AddScoped<IProductImageDeleter, SupabaseProductImageDeleter>();
        return services;
    }
}
