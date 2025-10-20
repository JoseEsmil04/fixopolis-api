using MediatR;

namespace Fixopolis.Application.Orders.Commands.MarkOrderPaid;


public sealed record MarkOrderPaidCommand(Guid OrderId) : IRequest<bool>;