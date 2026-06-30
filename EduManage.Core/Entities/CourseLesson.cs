// CourseLesson.cs
using EduManage.Core.Enums;

namespace EduManage.Core.Entities;

public class CourseLesson : BaseEntity
{
    public int SectionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? VideoPath { get; set; }
    public string? VideoKey { get; set; }
    public VideoStatus VideoStatus { get; set; } = VideoStatus.Ready;
    public int DurationMinutes { get; set; } = 0;
    public bool IsFreePreview { get; set; } = false;
    public LessonStatus Status { get; set; } = LessonStatus.Draft;
    public float OrderIndex { get; set; } = 0;

    // Navigation
    public CourseSection Section { get; set; } = null!;
    public ICollection<LessonResource> Resources { get; set; } = [];
    public ICollection<QaQuestion> Questions { get; set; } = [];
    public ICollection<LessonProgress> Progresses { get; set; } = [];
}