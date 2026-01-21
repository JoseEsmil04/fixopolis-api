namespace Fixopolis.Application.Orders.Dtos;

public sealed record OrderItemDto(
    Guid ProductId,
    string ProductName,
    string ProductCode,
    decimal UnitPrice,
    int Quantity
);