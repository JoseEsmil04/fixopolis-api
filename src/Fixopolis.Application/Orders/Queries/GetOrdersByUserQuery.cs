using Fixopolis.Application.Orders.Dtos;
using MediatR;

namespace Fixopolis.Application.Orders.Queries.GetOrdersByUser;

public sealed record GetOrdersByUserQuery(Guid UserId) : IRequest<List<OrderDto>>;