namespace Fixopolis.Application.Orders.Dtos;

public sealed record OrderCreateDto(Guid UserId, List<OrderItemCreateDto> Items);