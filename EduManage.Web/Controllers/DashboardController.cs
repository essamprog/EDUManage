using EduManage.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize(Roles = "Student")]
public class DashboardController : Controller
{
    private readonly IEnrollmentService _enrollmentService;

    public DashboardController(IEnrollmentService enrollmentService)
        => _enrollmentService = enrollmentService;

    private int UserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<IActionResult> Index()
    {
        var enrollments = await _enrollmentService.GetStudentEnrollmentsAsync(UserId);
        return View(enrollments);
    }
}