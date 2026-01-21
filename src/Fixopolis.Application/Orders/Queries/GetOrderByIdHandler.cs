using Fixopolis.Application.Abstractions;
using Fixopolis.Application.Orders.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixopolis.Application.Orders.Queries.GetOrderById;

public sealed class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, OrderDto?>
{
    private readonly IAppDbContext _db;

    public GetOrderByIdHandler(IAppDbContext db) => _db = db;

    public async Task<OrderDto?> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _db.Orders
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

        if (order == null)
        {
            return null;
        }

        var itemsDto = order.Items!.Select(i => new OrderItemDto(
            i.ProductId,
            i.Product!.Name!,
            i.Product.Code!,
            i.UnitPrice,
            i.Quantity
        )).ToList();

        return new OrderDto(
            order.Id,
            order.UserId,
            order.CreatedAt,
            order.Status.ToString(),
            order.Total,
            itemsDto
        );
    }
}