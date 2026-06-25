// QaQuestion.cs
namespace EduManage.Core.Entities;

public class QaQuestion : BaseEntity
{
    public int LessonId { get; set; }
    public int UserId { get; set; }
    public int? ParentId { get; set; }
    public string QuestionText { get; set; } = string.Empty;

    // Navigation
    public CourseLesson Lesson { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
    public QaQuestion? Parent { get; set; }
    public ICollection<QaQuestion> Replies { get; set; } = [];
}