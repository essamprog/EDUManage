using System;
using System.Collections.Generic;
using System.Text;

// Courses/CourseDto.cs
namespace EduManage.Application.DTOs.Courses;

public class CourseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string? ThumbnailUrl { get; set; }
    public decimal Price { get; set; }
    public decimal AverageRating { get; set; }
    public int TotalStudents { get; set; }
    public int TotalLessons { get; set; }
    public string Level { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string InstructorName { get; set; } = string.Empty;
    public string? CategoryName { get; set; }
    public bool IsBestseller { get; set; }
}