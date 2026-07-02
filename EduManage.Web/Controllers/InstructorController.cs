using EduManage.Application.DTOs.Courses;
using EduManage.Application.DTOs.Financial;
using EduManage.Application.Interfaces;
using EduManage.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Authorize(Roles = "Instructor")]
public class InstructorController : Controller
{
    private readonly ICourseService _courseService;
    private readonly IWalletService _walletService;
    private readonly IUnitOfWork _uow;

    public InstructorController(
        ICourseService courseService,
        IWalletService walletService,
        IUnitOfWork uow)
    {
        _courseService = courseService;
        _walletService = walletService;
        _uow = uow;
    }

    private int UserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // GET /instructor
    public async Task<IActionResult> Index()
    {
        var courses = await _courseService.GetByInstructorAsync(UserId);
        var wallet = await _walletService.GetWalletAsync(UserId);

        ViewData["Courses"] = courses;
        ViewData["TotalCourses"] = courses.Count();
        ViewData["TotalStudents"] = courses.Sum(c => c.TotalStudents);
        ViewData["AvailableBalance"] = wallet.AvailableBalance;
        ViewData["LifetimeEarnings"] = wallet.LifetimeEarnings;

        return View();
    }

    // GET /instructor/courses
    public async Task<IActionResult> Courses()
    {
        var courses = await _courseService.GetByInstructorAsync(UserId);
        return View(courses);
    }

    // GET /instructor/create
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewData["Categories"] = await _uow.Categories.GetAllAsync();
        return View(new CreateCourseDto());
    }

    // POST /instructor/create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCourseDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Categories"] = await _uow.Categories.GetAllAsync();
            return View(dto);
        }

        await _courseService.CreateAsync(UserId, dto);
        TempData["Success"] = "Course created successfully.";
        return RedirectToAction("Courses");
    }

    // GET /instructor/edit/5
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var course = await _courseService.GetByIdAsync(id);
        if (course is null) return NotFound();

        ViewData["Categories"] = await _uow.Categories.GetAllAsync();
        return View(new UpdateCourseDto
        {
            Title = course.Title,
            Subtitle = course.Subtitle,
            Description = course.Description,
            Price = course.Price,
            CategoryId = course.CategoryId,
        });
    }

    // POST /instructor/edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, UpdateCourseDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Categories"] = await _uow.Categories.GetAllAsync();
            return View(dto);
        }

        await _courseService.UpdateAsync(id, dto);
        TempData["Success"] = "Course updated successfully.";
        return RedirectToAction("Courses");
    }

    // POST /instructor/publish/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Publish(int id)
    {
        await _courseService.PublishAsync(id);
        TempData["Success"] = "Course published.";
        return RedirectToAction("Courses");
    }

    // POST /instructor/delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _courseService.DeleteAsync(id);
        TempData["Success"] = "Course deleted.";
        return RedirectToAction("Courses");
    }

    // GET /instructor/financials
    public async Task<IActionResult> Financials()
    {
        var wallet = await _walletService.GetWalletAsync(UserId);
        return View(wallet);
    }

    // POST /instructor/withdraw
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Withdraw(WithdrawalDto dto)
    {
        try
        {
            await _walletService.RequestWithdrawalAsync(UserId, dto);
            TempData["Success"] = "Withdrawal request sent.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction("Financials");
    }
}