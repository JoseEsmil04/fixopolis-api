using Fixopolis.Application.Abstractions;

namespace Fixopolis.WebApi.Infrastructure.Images;

public sealed class FileSystemProductImageDeleter : IProductImageDeleter
{
    private readonly IWebHostEnvironment _env;
    public FileSystemProductImageDeleter(IWebHostEnvironment env)
    {
        _env = env;
    }

    public Task DeleteImageAsync(string? relativeUrl, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(relativeUrl)) return Task.CompletedTask;

        var webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
        var normalized = relativeUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var full = Path.Combine(webRoot, normalized);
        if (File.Exists(full)) File.Delete(full);
        return Task.CompletedTask;
    }
}
