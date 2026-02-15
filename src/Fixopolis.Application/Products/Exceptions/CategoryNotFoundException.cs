namespace Fixopolis.Application.Products.Exceptions;

public sealed class CategoryNotFoundException(Guid categoryId)
    : Exception($"La categoría con ID '{categoryId}' no existe.")
{
    public Guid CategoryId { get; } = categoryId;
}
