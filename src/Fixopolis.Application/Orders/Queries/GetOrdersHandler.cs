using Fixopolis.Application.Abstractions;
using Fixopolis.Application.Orders.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixopolis.Application.Orders.Queries.GetOrders;

public sealed class GetOrdersHandler : IRequestHandler<GetOrdersQuery, List<OrderDto>>
{
    private readonly IAppDbContext _db;

    public GetOrdersHandler(IAppDbContext db) => _db = db;

    public async Task<List<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _db.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .AsNoTracking()
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);

        return orders.Select(order => new OrderDto(
            order.Id,
            order.UserId,
            order.CreatedAt,
            order.Status.ToString(),
            order.Total,
            order.Items.Select(i => new OrderItemDto(
                i.ProductId,
                i.Product!.Name!,
                i.Product.Code!,
                i.UnitPrice,
                i.Quantity
            )).ToList()
        )).ToList();
    }
}