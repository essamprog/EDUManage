using EduManage.Core.Enums;

namespace EduManage.Application.DTOs.Courses
{
    public class CourseSectionDto
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public float OrderIndex { get; set; }
        public List<CourseLessonDto> Lessons { get; set; } = new();
    }

    public class CourseLessonDto
    {
        public int Id { get; set; }
        public int SectionId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? VideoPath { get; set; }
        public string? VideoKey { get; set; }
        public int DurationMinutes { get; set; }
        public bool IsFreePreview { get; set; }
        public LessonStatus Status { get; set; }
        public float OrderIndex { get; set; }
    }

    public class CreateLessonDto
    {
        public string Title { get; set; } = string.Empty;
        public string? VideoPath { get; set; }
        public string? VideoKey { get; set; }
        public int DurationMinutes { get; set; }
        public bool IsFreePreview { get; set; }
    }
}
