// Financial/WalletDto.cs
namespace EduManage.Application.DTOs.Financial;

public class WalletDto
{
    public decimal AvailableBalance { get; set; }
    public decimal PendingBalance { get; set; }
    public decimal TotalWithdrawn { get; set; }
    public decimal LifetimeEarnings { get; set; }

    // Detailed records
    public List<TransactionRecordDto> Transactions { get; set; } = new();
    public List<WithdrawalRecordDto> Withdrawals { get; set; } = new();
}

public class TransactionRecordDto
{
    public int Id { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal NetAmount { get; set; }
    public decimal PlatformFee => GrossAmount - NetAmount;
    public string? CourseName { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class WithdrawalRecordDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;
    public string AccountDetail { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? InstructorName { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? PaidAt { get; set; }
}