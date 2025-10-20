using System;
using Fixopolis.Application.Orders.Dtos;
using MediatR;

namespace Fixopolis.Application.Orders.Queries.GetOrderById;

public sealed record GetOrderByIdQuery(Guid OrderId) : IRequest<OrderDto?>;