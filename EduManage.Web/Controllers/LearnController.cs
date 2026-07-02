using EduManage.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduManage.Web.Controllers;

[Authorize(Roles = "Student")]
public class LearnController : Controller
{
    private readonly ICourseService _courseService;
    private readonly IEnrollmentService _enrollmentService;

    public LearnController(ICourseService courseService, IEnrollmentService enrollmentService)
    {
        _courseService = courseService;
        _enrollmentService = enrollmentService;
    }

    private int UserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // GET: /Learn/Index?courseId=5
    public async Task<IActionResult> Index(int courseId, int? lessonId = null)
    {
        // 1. Check if user is enrolled
        var enrollments = await _enrollmentService.GetStudentEnrollmentsAsync(UserId);
        if (!enrollments.Any(e => e.CourseId == courseId))
        {
            TempData["Error"] = "You do not have access to this course.";
            return RedirectToAction("Index", "Dashboard");
        }

        // 2. Fetch the course with its curriculum
        var course = await _courseService.GetByIdAsync(courseId);
        if (course == null) return NotFound();

        // 3. Find the requested lesson or default to the very first one
        ViewData["ActiveLessonId"] = lessonId;

        return View(course);
    }
}
