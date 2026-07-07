using EduManage.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduManage.Web.Controllers;

[Authorize]
public class NotificationsController : Controller
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
        => _notificationService = notificationService;

    private int UserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // GET /Notifications
    public async Task<IActionResult> Index()
    {
        var notifications = await _notificationService.GetUserNotificationsAsync(UserId);
        return View(notifications);
    }

    // POST /Notifications/MarkRead?id=5
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkRead(int id)
    {
        await _notificationService.MarkAsReadAsync(id);
        return RedirectToAction(nameof(Index));
    }

    // POST /Notifications/MarkAllRead
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAllRead()
    {
        await _notificationService.MarkAllAsReadAsync(UserId);
        return RedirectToAction(nameof(Index));
    }

    // GET /Notifications/Dropdown  — called via AJAX by the navbar
    [HttpGet]
    public async Task<IActionResult> Dropdown()
    {
        var notifications = await _notificationService.GetUserNotificationsAsync(UserId);
        return PartialView("_NotificationsDropdown", notifications);
    }
}
