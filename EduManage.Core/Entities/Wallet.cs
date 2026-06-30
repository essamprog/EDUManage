// Wallet.cs
namespace EduManage.Core.Entities;

public class Wallet : BaseEntity
{
    public int InstructorId { get; set; }
    public decimal AvailableBalance { get; set; } = 0;
    public decimal PendingBalance { get; set; } = 0;
    public decimal TotalWithdrawn { get; set; } = 0;
    public decimal LifetimeEarnings { get; set; } = 0;

    // Navigation
    public InstructorProfile Instructor { get; set; } = null!;
    public ICollection<Withdrawal> Withdrawals { get; set; } = [];
}