using EduManage.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

// Courses/CreateCourseDto.cs
namespace EduManage.Application.DTOs.Courses;

public class CreateCourseDto
{
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string? Description { get; set; }
    public CourseLevel Level { get; set; }
    public int? CategoryId { get; set; }
    public decimal Price { get; set; }
    public string Language { get; set; } = "Arabic";
}
