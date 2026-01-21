using Fixopolis.Application.Abstractions;
using Fixopolis.Application.Orders.Dtos;
using Fixopolis.Domain.Entities;
using Fixopolis.Domain.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Fixopolis.Application.Orders.Commands.CreateOrder;

public sealed class CreateOrderHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly IAppDbContext _db;

    public CreateOrderHandler(IAppDbContext db) => _db = db;

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var body = request.Body;
        var distinctProductIds = body.Items.Select(i => i.ProductId).Distinct().ToList();

        var userExists = await _db.Users.AnyAsync(u => u.Id == body.UserId, cancellationToken);
        if (!userExists)
        {
            throw new InvalidOperationException("El usuario especificado no existe.");
        }

        var products = await _db.Products
            .Where(p => distinctProductIds.Contains(p.Id) && p.IsAvailable)
            .ToListAsync(cancellationToken);

        if (products.Count != distinctProductIds.Count)
        {
            throw new InvalidOperationException("Uno o más productos no existen o no están disponibles.");
        }

        var orderItems = new List<OrderItem>();
        foreach (var item in body.Items)
        {
            var product = products.First(p => p.Id == item.ProductId);
            if (product.Stock < item.Quantity)
            {
                throw new InvalidOperationException($"Stock insuficiente para el producto {product.Code}.");
            }

            product.Stock -= item.Quantity;

            var orderItem = new OrderItem
            {
                ProductId = product.Id,
                Quantity = item.Quantity,
                UnitPrice = product.Price,
                LineTotal = product.Price * item.Quantity
            };

            orderItems.Add(orderItem);
        }

        var orderTotal = orderItems.Sum(i => i.LineTotal);

        var order = new Order
        {
            UserId = body.UserId,
            CreatedAt = DateTime.UtcNow,
            Total = orderTotal,
            Status = OrderStatus.Pending,
            Items = orderItems
        };

        _db.Orders.Add(order);
        await _db.SaveChangesAsync(cancellationToken);

        var itemsDto = orderItems.Select(i =>
        {
            var product = products.First(p => p.Id == i.ProductId);
            return new OrderItemDto(
                i.ProductId,
                product.Name!,
                product.Code!,
                i.UnitPrice,
                i.Quantity
            );
        }).ToList();

        var orderDto = new OrderDto(
            order.Id,
            order.UserId,
            order.CreatedAt,
            order.Status.ToString(),
            order.Total,
            itemsDto
        );

        return orderDto;
    }
}