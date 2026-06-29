using EduManage.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

// Courses/CourseFilterDto.cs
namespace EduManage.Application.DTOs.Courses;

public class CourseFilterDto
{
    public string? Search { get; set; }
    public int? CategoryId { get; set; }
    public CourseLevel? Level { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string SortBy { get; set; } = "newest";
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}