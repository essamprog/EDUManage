using System;
using System.Collections.Generic;
using System.Text;

// Financial/WithdrawalRequestDto.cs
namespace EduManage.Application.DTOs.Financial;

public class WithdrawalRequestDto
{
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;
    public string AccountDetail { get; set; } = string.Empty;
}