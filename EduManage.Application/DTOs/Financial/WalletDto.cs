using System;
using System.Collections.Generic;
using System.Text;

// Financial/WalletDto.cs
namespace EduManage.Application.DTOs.Financial;

public class WalletDto
{
    public decimal AvailableBalance { get; set; }
    public decimal PendingBalance { get; set; }
    public decimal TotalWithdrawn { get; set; }
    public decimal LifetimeEarnings { get; set; }
}