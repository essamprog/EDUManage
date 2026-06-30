// Withdrawal.cs
using EduManage.Core.Enums;

namespace EduManage.Core.Entities;

public class Withdrawal : BaseEntity
{
    public int InstructorId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EGP";
    public string Method { get; set; } = string.Empty;
    public string AccountDetail { get; set; } = string.Empty;
    public WithdrawalStatus Status { get; set; } = WithdrawalStatus.Pending;
    public DateTime RequestedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public InstructorProfile Instructor { get; set; } = null!;
}