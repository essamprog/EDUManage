using System;

namespace EduManage.Application.DTOs.Courses
{
    public class UpdateCourseDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public int? CategoryId { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? Subtitle { get; set; }
    }
}