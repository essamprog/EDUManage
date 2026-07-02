using EduManage.Application.DTOs.Courses;
using EduManage.Application.Interfaces;
using EduManage.Core.Enums;
using EduManage.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduManage.Web.Controllers;

public class CoursesController : Controller
{
    private readonly ICourseService _courseService;
    private readonly IUnitOfWork _uow;

    public CoursesController(ICourseService courseService, IUnitOfWork uow)
    {
        _courseService = courseService;
        _uow = uow;
    }

    // ── Browse ────────────────────────────────────────────
    public async Task<IActionResult> Index(
        string? search,
        int? categoryId,
        string? level,
        string sortBy = "newest",
        int page = 1)
    {
        var filter = new CourseFilterDto
        {
            Search = search,
            CategoryId = categoryId,
            SortBy = sortBy,
            Page = page,
            PageSize = 12,
        };

        if (Enum.TryParse<CourseLevel>(level, out var parsedLevel))
            filter.Level = parsedLevel;

        var result = await _courseService.GetAllAsync(filter);

        // بيانات الـ Sidebar
        ViewData["Categories"] = await _uow.Categories.GetAllAsync();
        ViewData["Search"] = search;
        ViewData["CategoryId"] = categoryId;
        ViewData["Level"] = level;
        ViewData["SortBy"] = sortBy;

        return View(result);
    }

    // ── Details ───────────────────────────────────────────
    public async Task<IActionResult> Details(int id)
    {
        var course = await _courseService.GetByIdAsync(id);
        if (course is null) return NotFound();
        return View(course);
    }
}