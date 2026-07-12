using EduManage.Application.DTOs.Courses;
using EduManage.Application.Interfaces;
using EduManage.Core.Enums;
using EduManage.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduManage.Web.Controllers;

public class CoursesController : Controller
{
    private readonly ICourseService _courseService;
    private readonly IUnitOfWork _uow;
    private readonly IOrderService _orderService;

    public CoursesController(ICourseService courseService, IUnitOfWork uow, IOrderService orderService)
    {
        _courseService = courseService;
        _uow = uow;
        _orderService = orderService;
    }

    // ── Browse ────────────────────────────────────────────
    public async Task<IActionResult> Index(
        string? search,
        int? categoryId,
        int? instructorId,
        string? level,
        string sortBy = "newest",
        int page = 1)
    {
        var filter = new CourseFilterDto
        {
            Search = search,
            CategoryId = categoryId,
            InstructorId = instructorId,
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
        ViewData["InstructorId"] = instructorId;
        ViewData["Level"] = level;
        ViewData["SortBy"] = sortBy;

        return View(result);
    }

    // ── Details ───────────────────────────────────────────
    public async Task<IActionResult> Details(int id)
    {
        var course = await _courseService.GetByIdAsync(id);
        if (course is null) return NotFound();

        // Check cart & enrollment status for authenticated users
        if (User.Identity?.IsAuthenticated == true)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out var userId))
            {
                var isInCart = await _uow.CartItems
                    .AnyAsync(c => c.StudentId == userId && c.CourseId == id);

                var isEnrolled = await _uow.Enrollments
                    .AnyAsync(e => e.StudentId == userId && e.CourseId == id);

                ViewData["IsInCart"]   = isInCart;
                ViewData["IsEnrolled"] = isEnrolled;
            }
        }

        return View(course);
    }
}