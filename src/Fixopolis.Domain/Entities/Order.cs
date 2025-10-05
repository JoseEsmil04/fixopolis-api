using System;
using Fixopolis.Domain.Common;

namespace Fixopolis.Domain.Entities;

public class Order : BaseEntity
{
    public Guid UserId { get; set; }
    public User? User { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal Total { get; set; }

    public List<OrderItem>? Items { get; set; }
}