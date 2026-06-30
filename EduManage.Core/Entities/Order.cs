// Order.cs
using EduManage.Core.Enums;

namespace EduManage.Core.Entities;

public class Order : BaseEntity
{
    public int StudentId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal Discount { get; set; } = 0;
    public string? CouponCode { get; set; }
    public string PaymentMethod { get; set; } = "paymob";
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public string? TransactionId { get; set; }

    // Navigation
    public ApplicationUser Student { get; set; } = null!;
    public ICollection<OrderItem> Items { get; set; } = [];
}