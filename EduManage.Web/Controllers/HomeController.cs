using EduManage.Application.DTOs.Courses;
using EduManage.Application.Interfaces;
using EduManage.Core.Enums;
using EduManage.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduManage.Web.Controllers;

public class HomeController : Controller
{
    private readonly ICourseService _courseService;

    public HomeController(ICourseService courseService)
        => _courseService = courseService;

    public async Task<IActionResult> Index()
    {
        var featured = await _courseService.GetAllAsync(new CourseFilterDto
        {
            SortBy = "popular",
            Page = 1,
            PageSize = 6,
        });

        return View(featured.Items);
    }

    public IActionResult Error() => View();
}