using Fixopolis.Application.Abstractions;
using Fixopolis.Domain.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixopolis.Application.Orders.Commands.CancelOrder;

public sealed class CancelOrderHandler : IRequestHandler<CancelOrderCommand, bool>
{
    private readonly IAppDbContext _db;

    public CancelOrderHandler(IAppDbContext db) => _db = db;

    public async Task<bool> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _db.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order == null)
        {
            return false;
        }

        if (order.Status == OrderStatus.Cancelled)
        {
            return true;
        }

        foreach (var item in order.Items!)
        {
            var product = await _db.Products.FirstOrDefaultAsync(p => p.Id == item.ProductId, cancellationToken);
            if (product != null)
            {
                product.Stock += item.Quantity;
            }
        }

        order.Status = OrderStatus.Cancelled;
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}