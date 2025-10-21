using Microsoft.Extensions.DependencyInjection;
using Fixopolis.Application.Abstractions;
using Fixopolis.WebApi.Infrastructure.Images;

namespace Fixopolis.WebApi.Infrastructure.Images;

public static class ProductImageServicesRegistration
{
    public static IServiceCollection AddProductImageServices(this IServiceCollection services)
    {
        services.AddScoped<IProductImageStorage, FileSystemProductImageStorage>();
        services.AddScoped<IProductImageDeleter, FileSystemProductImageDeleter>();
        return services;
    }
}
