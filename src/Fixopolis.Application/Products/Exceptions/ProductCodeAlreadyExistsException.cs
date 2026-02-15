namespace Fixopolis.Application.Products.Exceptions;

public sealed class ProductCodeAlreadyExistsException(string code)
    : Exception($"El código de producto '{code}' ya está en uso.")
{
    public string Code { get; } = code;
}
