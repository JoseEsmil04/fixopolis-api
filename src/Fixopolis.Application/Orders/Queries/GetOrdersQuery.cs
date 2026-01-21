using System.Collections.Generic;
using Fixopolis.Application.Orders.Dtos;
using MediatR;

namespace Fixopolis.Application.Orders.Queries.GetOrders;

public sealed record GetOrdersQuery() : IRequest<List<OrderDto>>;