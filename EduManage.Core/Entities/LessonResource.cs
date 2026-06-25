// LessonResource.cs
namespace EduManage.Core.Entities;

public class LessonResource : BaseEntity
{
    public int LessonId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string? FileKey { get; set; }
    public string FileType { get; set; } = string.Empty;

    // Navigation
    public CourseLesson Lesson { get; set; } = null!;
}