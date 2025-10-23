using Fixopolis.Application.Abstractions;

namespace Fixopolis.WebApi.Infrastructure.Images;

public sealed class SupabaseProductImageDeleter : IProductImageDeleter
{
    private readonly Supabase.Client _client;
    private const string BucketName = "product-images";

    public SupabaseProductImageDeleter(Supabase.Client client) => _client = client;

    public async Task DeleteImageAsync(string? imageUrl, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(imageUrl)) return;

        try
        {
            var uri = new Uri(imageUrl);
            var parts = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var publicIdx = Array.IndexOf(parts, "public");
            if (publicIdx < 0 || publicIdx + 1 >= parts.Length) return;

            var bucket = parts[publicIdx + 1];
            if (!string.Equals(bucket, BucketName, StringComparison.Ordinal))
            {
                return;
            }

            var keyStart = publicIdx + 2;
            if (keyStart >= parts.Length) return;

            var objectPath = string.Join('/', parts[keyStart..]); // relativo al bucket, ej: "folder/archivo.webp"
            var bucketRef = _client.Storage.From(BucketName);
            await bucketRef.Remove(objectPath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ [SupabaseProductImageDeleter] No se pudo eliminar la imagen: {ex.Message}");
        }

    }
}
