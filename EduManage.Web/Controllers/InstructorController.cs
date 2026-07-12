using EduManage.Application.Common;
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
    private readonly IPhotoService _photoService;
    private readonly IUnitOfWork _uow;

    public InstructorController(
        ICourseService courseService,
        IWalletService walletService,
        IPhotoService photoService,
        IUnitOfWork uow)
    {
        _courseService = courseService;
        _walletService = walletService;
        _photoService = photoService;
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

    // ==========================================
    // WIZARD STEP 1: Basic Info
    // ==========================================
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewData["Categories"] = await _uow.Categories.GetAllAsync();
        return View(new CreateCourseDto());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCourseDto dto)
    {
        if (!ModelState.IsValid)
        {
            ViewData["Categories"] = await _uow.Categories.GetAllAsync();
            return View(dto);
        }

        var course = await _courseService.CreateAsync(UserId, dto);
        return RedirectToAction("Curriculum", new { id = course.Id });
    }

    // ==========================================
    // WIZARD STEP 2: Curriculum (Sections & Lessons)
    // ==========================================
    [HttpGet]
    public async Task<IActionResult> Curriculum(int id)
    {
        var course = await _courseService.GetByIdAsync(id);
        if (course == null || course.InstructorId != UserId) return NotFound();

        return View(course);
    }

    [HttpPost]
    public async Task<IActionResult> AddSection(int courseId, string title)
    {
        var course = await _courseService.GetByIdAsync(courseId);
        if (course == null || course.InstructorId != UserId) return Unauthorized();

        var section = await _courseService.AddSectionAsync(courseId, title);
        return Json(new { success = true, section });
    }

    [HttpPost]
    [RequestSizeLimit(10L * 1024 * 1024 * 1024)] // Allow large uploads
    [RequestFormLimits(MultipartBodyLengthLimit = 10L * 1024 * 1024 * 1024)]
    public async Task<IActionResult> AddLesson(int sectionId, string title, bool isFreePreview, IFormFile videoFile)
    {
        var section = await _uow.CourseSections.GetByIdAsync(sectionId);
        if (section == null) return NotFound();
        
        var course = await _courseService.GetByIdAsync(section.CourseId);
        if (course == null || course.InstructorId != UserId) return Unauthorized();

        string folderPath = StoragePaths.CourseVideos(UserId, course.Id);
        var uploadResult = await _photoService.AddVideoAsync(videoFile, folderPath);

        var lessonDto = new CreateLessonDto
        {
            Title = title,
            VideoPath = uploadResult.Url,
            VideoKey = uploadResult.PublicId,
            DurationMinutes = (uploadResult.Duration / 60), // Convert seconds to minutes
            IsFreePreview = isFreePreview
        };

        var lesson = await _courseService.AddLessonAsync(sectionId, lessonDto);
        return Json(new { success = true, lesson });
    }

    // ==========================================
    // WIZARD STEP 3: Media & Publish
    // ==========================================
    [HttpGet]
    public async Task<IActionResult> MediaAndPublish(int id)
    {
        var course = await _courseService.GetByIdAsync(id);
        if (course == null || course.InstructorId != UserId) return NotFound();

        return View(course);
    }

    [HttpPost]
    public async Task<IActionResult> UploadThumbnail(int courseId, IFormFile imageFile)
    {
        var course = await _courseService.GetByIdAsync(courseId);
        if (course == null || course.InstructorId != UserId) return Unauthorized();

        if (imageFile != null && imageFile.Length > 0)
        {
            string folderPath = StoragePaths.CourseImages(UserId, course.Id);
            var uploadResult = await _photoService.AddImageAsync(imageFile, folderPath);
            await _courseService.UpdateMediaAsync(courseId, uploadResult.Url, uploadResult.PublicId, null, null);
        }
        
        return RedirectToAction("MediaAndPublish", new { id = courseId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Publish(int id)
    {
        var course = await _courseService.GetByIdAsync(id);
        if (course == null || course.InstructorId != UserId) return Unauthorized();

        await _courseService.PublishAsync(id);
        TempData["Success"] = "Course published successfully.";
        return RedirectToAction("Courses");
    }

    // ==========================================
    // EDIT & DELETE
    // ==========================================
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _courseService.DeleteAsync(id);
        TempData["Success"] = "Course deleted.";
        return RedirectToAction("Courses");
    }

    // ==========================================
    // FINANCIALS
    // ==========================================
    public async Task<IActionResult> Financials()
    {
        var wallet = await _walletService.GetFullWalletAsync(UserId);
        return View(wallet);
    }

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