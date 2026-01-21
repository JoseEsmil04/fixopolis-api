using Fixopolis.Application.Abstractions;
using Fixopolis.Domain.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixopolis.Application.Orders.Commands.MarkOrderPaid;

public sealed class MarkOrderPaidHandler : IRequestHandler<MarkOrderPaidCommand, bool>
{
    private readonly IAppDbContext _db;

    public MarkOrderPaidHandler(IAppDbContext db) => _db = db;

    public async Task<bool> Handle(MarkOrderPaidCommand request, CancellationToken cancellationToken)
    {
        var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);
        if (order == null)
        {
            return false;
        }

        if (order.Status == OrderStatus.Cancelled)
        {
            throw new InvalidOperationException("No se puede pagar una orden cancelada.");
        }

        if (order.Status == OrderStatus.Paid)
        {
            return true;
        }

        order.Status = OrderStatus.Paid;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}