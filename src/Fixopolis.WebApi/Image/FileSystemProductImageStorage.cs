namespace Fixopolis.WebApi.Infrastructure.Images;

public sealed class FileSystemProductImageStorage : IProductImageStorage
{
    private readonly IWebHostEnvironment _env;
    private const long MaxBytes = 2 * 1024 * 1024; // 2MB
    private static readonly string[] Allowed = [".jpg", ".jpeg", ".png", ".webp"];

    public FileSystemProductImageStorage(IWebHostEnvironment env) => _env = env;

    public async Task<string?> SaveImageAsync(IFormFile? file, CancellationToken ct = default)
    {
        if (file is null || file.Length == 0) return null;
        if (file.Length > MaxBytes) throw new InvalidOperationException("Imagen excede 2MB.");
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!Allowed.Contains(ext)) throw new InvalidOperationException("Extensión no permitida.");

        var webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
        var dir = Path.Combine(webRoot, "images", "products");
        Directory.CreateDirectory(dir);

        var name = $"{Guid.NewGuid():N}{ext}";
        var full = Path.Combine(dir, name);

        await using var stream = File.Create(full);
        await file.CopyToAsync(stream, ct);

        return $"/images/products/{name}";
    }
}
