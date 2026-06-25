// LessonProgress.cs
namespace EduManage.Core.Entities;

public class LessonProgress : BaseEntity
{
    public int StudentId { get; set; }
    public int LessonId { get; set; }
    public int CourseId { get; set; }
    public bool IsCompleted { get; set; } = false;
    public int WatchTime { get; set; } = 0;   // بالثواني
    public DateTime? CompletedAt { get; set; }

    // Navigation
    public ApplicationUser Student { get; set; } = null!;
    public CourseLesson Lesson { get; set; } = null!;
    public Course Course { get; set; } = null!;
}