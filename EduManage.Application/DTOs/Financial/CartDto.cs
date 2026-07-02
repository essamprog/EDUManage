namespace EduManage.Application.DTOs.Financial;

public class CartDto
{
    public IEnumerable<CartItemDto> Items { get; set; } = [];
    public decimal TotalPrice { get; set; }
    public decimal Discount { get; set; } = 0;
    public decimal FinalPrice => TotalPrice - Discount;
}