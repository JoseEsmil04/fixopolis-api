using Fixopolis.Application.Orders.Dtos;
using MediatR;

namespace Fixopolis.Application.Orders.Commands.CreateOrder;

public sealed record CreateOrderCommand(OrderCreateDto Body) : IRequest<OrderDto>;