namespace Fixopolis.Application.Abstractions;

public interface IProductImageDeleter
{
    Task DeleteImageAsync(string? relativeUrl, CancellationToken ct = default);
}
