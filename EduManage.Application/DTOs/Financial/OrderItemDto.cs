namespace EduManage.Application.DTOs.Financial;

public class OrderItemDto
{
    public int CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public decimal Price { get; set; }
}