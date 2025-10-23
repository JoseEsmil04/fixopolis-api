

// This implementation of IProductImageStorage uses Supabase Storage instead of the local file system.
// It uploads images to a designated bucket and returns the public URL to be stored on the Product entity.
namespace Fixopolis.WebApi.Infrastructure.Images;

public sealed class SupabaseProductImageStorage : IProductImageStorage
{
    private readonly Supabase.Client _client;
    // Maximum allowed size for product images (2 MB).
    private const long MaxBytes = 2 * 1024 * 1024;
    // Allowed file extensions for images.
    private static readonly string[] Allowed = [".jpg", ".jpeg", ".png", ".webp"];
    // Name of the bucket to store product images. Configure this in Supabase.
    private const string BucketName = "product-images";

    public SupabaseProductImageStorage(Supabase.Client client)
    {
        _client = client;
    }

    public async Task<string?> SaveImageAsync(IFormFile? file, CancellationToken ct = default)
    {
        if (file is null || file.Length == 0)
            return null;

        if (file.Length > MaxBytes)
            throw new InvalidOperationException("Imagen excede 2MB.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!Allowed.Contains(ext))
            throw new InvalidOperationException("Extensión no permitida.");

        await using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream, ct);
        var bytes = memoryStream.ToArray();

        var fileName = $"{Guid.NewGuid():N}{ext}";
        var bucket = _client.Storage.From(BucketName);
        await bucket.Upload(bytes, fileName);

        var url = bucket.GetPublicUrl(fileName);
        return url;
    }
}