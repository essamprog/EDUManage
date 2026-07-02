namespace EduManage.Application.DTOs.Financial;

public class OrderDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal Discount { get; set; }
    public string? CouponCode { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? TransactionId { get; set; }
    public DateTime CreatedAt { get; set; }
    public IEnumerable<OrderItemDto> Items { get; set; } = [];
}