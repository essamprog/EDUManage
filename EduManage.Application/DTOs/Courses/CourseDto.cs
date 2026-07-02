namespace EduManage.Application.DTOs.Courses;

public class CourseDto
{
    public int Id { get; set; }
    public int InstructorId { get; set; }      // ← مضاف
    public int? CategoryId { get; set; }      // ← مضاف
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string? Description { get; set; }      // ← مضاف
    public string? ThumbnailUrl { get; set; }
    public decimal Price { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalStudents { get; set; }
    public int TotalLessons { get; set; }
    public string Level { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;   // ← مضاف
    public string InstructorName { get; set; } = string.Empty;
    public string? CategoryName { get; set; }
    public bool IsBestseller { get; set; }
    public DateTime? PublishedAt { get; set; }      // ← مضاف
    public DateTime CreatedAt { get; set; }      // ← مضاف
}