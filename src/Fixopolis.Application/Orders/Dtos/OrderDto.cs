namespace Fixopolis.Application.Orders.Dtos;


public sealed record OrderDto(
    Guid Id,
    Guid UserId,
    DateTime CreatedAt,
    string Status,
    decimal Total,
    IReadOnlyCollection<OrderItemDto> Items
);