// CourseSection.cs
namespace EduManage.Core.Entities;

public class CourseSection : BaseEntity
{
    public int CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public float OrderIndex { get; set; } = 0;

    // Navigation
    public Course Course { get; set; } = null!;
    public ICollection<CourseLesson> Lessons { get; set; } = [];
}