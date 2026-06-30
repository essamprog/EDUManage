// Transaction.cs
using EduManage.Core.Enums;

namespace EduManage.Core.Entities;

public class Transaction : BaseEntity
{
    public int InstructorId { get; set; }
    public int? OrderItemId { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal NetAmount { get; set; }
    public string Currency { get; set; } = "EGP";
    public string? Description { get; set; }
    public TransactionType Type { get; set; } = TransactionType.Sale;
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;

    // Navigation
    public InstructorProfile Instructor { get; set; } = null!;
    public OrderItem? OrderItem { get; set; }
}