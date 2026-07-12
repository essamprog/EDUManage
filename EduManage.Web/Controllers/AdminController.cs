using EduManage.Application.Interfaces;
using EduManage.Core.Entities;
using EduManage.Core.Enums;
using EduManage.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IUnitOfWork _uow;
    private readonly ICourseService _courseService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IWalletService _walletService;

    public AdminController(
        IUnitOfWork uow,
        ICourseService courseService,
        UserManager<ApplicationUser> userManager,
        IWalletService walletService)
    {
        _uow = uow;
        _courseService = courseService;
        _userManager = userManager;
        _walletService = walletService;
    }

    // GET /admin
    public async Task<IActionResult> Index()
    {
        ViewData["TotalUsers"] = await _uow.Users.CountAsync();
        ViewData["TotalCourses"] = await _uow.Courses.CountAsync();
        ViewData["TotalEnrollments"] = await _uow.Enrollments.CountAsync();
        ViewData["TotalRevenue"] = (await _uow.Orders.FindAsync(
                                             o => o.Status == OrderStatus.Completed))
                                             .Sum(o => o.TotalAmount);
        ViewData["PendingCourses"] = await _uow.Courses.CountAsync(
                                             c => c.Status == CourseStatus.Pending);
        ViewData["PendingWithdrawals"] = await _uow.Withdrawals.CountAsync(
                                             w => w.Status == WithdrawalStatus.Pending);
        ViewData["RecentOrders"] = (await _uow.Orders.FindAsync(
                                             o => o.CreatedAt >= DateTime.UtcNow.AddDays(-7)))
                                             .OrderByDescending(o => o.CreatedAt).Take(5);
        return View();
    }

    // GET /admin/users
    public async Task<IActionResult> Users(string? search)
    {
        var users = await _uow.Users.GetAllAsync();

        if (!string.IsNullOrEmpty(search))
            users = users.Where(u =>
                u.FullName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                u.Email!.Contains(search, StringComparison.OrdinalIgnoreCase));

        var userList = users.ToList();

        // Fetch role for each user from real DB
        var userRoles = new Dictionary<int, string>();
        foreach (var u in userList)
        {
            var roles = await _userManager.GetRolesAsync(u);
            userRoles[u.Id] = roles.FirstOrDefault() ?? "Student";
        }

        ViewData["Search"] = search;
        ViewData["UserRoles"] = userRoles;
        return View(userList);
    }

    // POST /admin/ban
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Ban(int id)
    {
        var user = await _uow.Users.GetByIdAsync(id);
        if (user is null) return NotFound();

        user.Status = UserStatus.Banned;
        user.UpdatedAt = DateTime.UtcNow;
        _uow.Users.Update(user);
        await _uow.SaveChangesAsync();

        TempData["Success"] = $"'{user.FullName}' has been banned.";
        return RedirectToAction("Users");
    }

    // POST /admin/unban
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Unban(int id)
    {
        var user = await _uow.Users.GetByIdAsync(id);
        if (user is null) return NotFound();

        user.Status = UserStatus.Active;
        user.UpdatedAt = DateTime.UtcNow;
        _uow.Users.Update(user);
        await _uow.SaveChangesAsync();

        TempData["Success"] = $"'{user.FullName}' has been unbanned.";
        return RedirectToAction("Users");
    }

    // POST /admin/makeinstructor
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> MakeInstructor(int id)
    {
        var user = await _uow.Users.GetByIdAsync(id);
        if (user is null) return NotFound();

        await _userManager.RemoveFromRoleAsync(user, "Student");
        await _userManager.AddToRoleAsync(user, "Instructor");

        var exists = await _uow.InstructorProfiles.AnyAsync(ip => ip.UserId == id);
        if (!exists)
        {
            await _uow.InstructorProfiles.AddAsync(new InstructorProfile
            {
                UserId = id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            });
            await _uow.SaveChangesAsync();
        }

        TempData["Success"] = $"'{user.FullName}' is now an Instructor.";
        return RedirectToAction("Users");
    }

    // GET /admin/courses
    public async Task<IActionResult> Courses(string? search)
    {
        var all = await _uow.Courses.GetAllAsync();

        if (!string.IsNullOrEmpty(search))
            all = all.Where(c =>
                c.Title.Contains(search, StringComparison.OrdinalIgnoreCase));

        ViewData["Search"] = search;
        return View(all);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Archive(int id)
    {
        var course = await _uow.Courses.GetByIdAsync(id);
        if (course is null) return NotFound();

        course.Status = CourseStatus.Archived;
        course.UpdatedAt = DateTime.UtcNow;
        _uow.Courses.Update(course);
        await _uow.SaveChangesAsync();

        TempData["Success"] = "Course archived.";
        return RedirectToAction("Courses");
    }

    // GET /admin/pending
    public async Task<IActionResult> Pending()
    {
        var pending = await _uow.Courses.FindAsync(c => c.Status == CourseStatus.Pending);
        return View(pending);
    }

    // POST /admin/approve
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        await _courseService.PublishAsync(id);
        TempData["Success"] = "Course approved and published.";
        return RedirectToAction("Pending");
    }

    // POST /admin/reject
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id)
    {
        var course = await _uow.Courses.GetByIdAsync(id);
        if (course is null) return NotFound();

        course.Status = CourseStatus.Draft;
        course.UpdatedAt = DateTime.UtcNow;
        _uow.Courses.Update(course);
        await _uow.SaveChangesAsync();

        TempData["Error"] = "Course rejected.";
        return RedirectToAction("Pending");
    }

    // GET /admin/finance
    public async Task<IActionResult> Finance()
    {
        var withdrawals = await _uow.Withdrawals.Query()
            .Include(w => w.Instructor).ThenInclude(i => i.User)
            .Include(w => w.Instructor).ThenInclude(i => i.Wallet)
            .OrderByDescending(w => w.RequestedAt)
            .Take(20)
            .ToListAsync();

        var transactions = (await _uow.Transactions.GetAllAsync())
                               .OrderByDescending(t => t.CreatedAt).Take(10);

        var wallets = await _uow.Wallets.GetAllAsync();

        ViewData["TotalRevenue"] = (await _uow.Orders.FindAsync(
                                             o => o.Status == OrderStatus.Completed))
                                             .Sum(o => o.TotalAmount);
        ViewData["PlatformEarnings"] = (await _uow.Transactions.GetAllAsync())
                                             .Sum(t => t.GrossAmount - t.NetAmount);
        ViewData["InstructorsAvailableBalance"] = wallets.Sum(w => w.AvailableBalance);
        ViewData["InstructorsTotalWithdrawn"] = wallets.Sum(w => w.TotalWithdrawn);
        ViewData["Withdrawals"] = withdrawals;
        ViewData["RecentTransactions"] = transactions;

        return View();
    }

    // POST /admin/approvewithdrawal
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveWithdrawal(int id)
    {
        await _walletService.ApproveWithdrawalAsync(id);
        TempData["Success"] = "Withdrawal approved and instructor notified.";
        return RedirectToAction("Finance");
    }

    // POST /admin/rejectwithdrawal
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectWithdrawal(int id)
    {
        await _walletService.RejectWithdrawalAsync(id);
        TempData["Error"] = "Withdrawal rejected and instructor notified.";
        return RedirectToAction("Finance");
    }
}