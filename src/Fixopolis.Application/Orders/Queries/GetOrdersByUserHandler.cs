using Fixopolis.Application.Abstractions;
using Fixopolis.Application.Orders.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixopolis.Application.Orders.Queries.GetOrdersByUser;

public sealed class GetOrdersByUserHandler : IRequestHandler<GetOrdersByUserQuery, List<OrderDto>>
{
    private readonly IAppDbContext _db;

    public GetOrdersByUserHandler(IAppDbContext db) => _db = db;

    public async Task<List<OrderDto>> Handle(GetOrdersByUserQuery request, CancellationToken cancellationToken)
    {
        var userExists = await _db.Users.AnyAsync(u => u.Id == request.UserId, cancellationToken);
        if (!userExists) throw new KeyNotFoundException($"El Usuario con el ID: {request.UserId} no existe.");

        var orders = await _db.Orders
            .Where(o => o.UserId == request.UserId)
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