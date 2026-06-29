using System;

namespace EduManage.Application.DTOs.Courses
{
    public class EnrollmentDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public byte Progress { get; set; }
        public bool IsCompleted { get; set; }
        public string? CourseTitle { get; set; }
        public string? Status { get; set; }
    }
}