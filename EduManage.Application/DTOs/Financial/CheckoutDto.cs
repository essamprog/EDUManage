using System;
using System.Collections.Generic;
using System.Text;

// Financial/CheckoutDto.cs
namespace EduManage.Application.DTOs.Financial;

public class CheckoutDto
{
    public string? CouponCode { get; set; }
    public string PaymentMethod { get; set; } = "paymob";
    public string? TransactionId { get; set; }
}