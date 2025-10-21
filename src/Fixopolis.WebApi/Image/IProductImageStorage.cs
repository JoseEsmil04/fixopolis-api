namespace Fixopolis.WebApi;


public interface IProductImageStorage
{
    Task<string?> SaveImageAsync(IFormFile file, CancellationToken ct = default);
}