using MediatR;

namespace Fixopolis.Application.Orders.Commands.CancelOrder;


public sealed record CancelOrderCommand(Guid OrderId) : IRequest<bool>;