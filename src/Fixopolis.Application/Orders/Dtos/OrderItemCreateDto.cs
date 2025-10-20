namespace Fixopolis.Application.Orders.Dtos;


public sealed record OrderItemCreateDto(Guid ProductId, int Quantity);