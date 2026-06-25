// Review.cs
namespace EduManage.Core.Entities;

public class Review : BaseEntity
{
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public byte Rating { get; set; }   // 1-5
    public string? Comment { get; set; }

    // Navigation
    public ApplicationUser Student { get; set; } = null!;
    public Course Course { get; set; } = null!;
}