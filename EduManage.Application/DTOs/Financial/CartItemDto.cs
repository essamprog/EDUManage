namespace EduManage.Application.DTOs.Financial
{
    public class CartItemDto
    {
        public int CourseId { get; set; }
        public string? CourseTitle { get; set; }
        public decimal Price { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? Title { get; set; }
        public DateTime AddedAt { get; internal set; }
    }
}