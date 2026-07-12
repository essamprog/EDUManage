using EduManage.Application.Interfaces;
using EduManage.Core.Entities;
using EduManage.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace EduManage.Web.Filters;

/// <summary>
/// Injects real notification data AND the user's avatar URL into ViewData
/// for every authenticated request, so the navbar and dashboard sidebar
/// always show live data without touching each controller individually.
/// </summary>
public class NotificationActionFilter : IAsyncActionFilter
{
    private readonly INotificationService _notificationService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IOrderService _orderService;

    public NotificationActionFilter(
        INotificationService notificationService,
        UserManager<ApplicationUser> userManager,
        IOrderService orderService)
    {
        _notificationService = notificationService;
        _userManager = userManager;
        _orderService = orderService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Only for authenticated users with a controller that returns a View
        if (context.Controller is Controller controller &&
            controller.User.Identity?.IsAuthenticated == true)
        {
            var userIdClaim = controller.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdClaim, out var userId))
            {
                try
                {
                    // 1. Notifications for the navbar bell icon
                    var notifications = await _notificationService.GetUserNotificationsAsync(userId);
                    controller.ViewData["Notifications"] = notifications;

                    // 2. Profile picture for the navbar & dashboard sidebar
                    var user = await _userManager.FindByIdAsync(userId.ToString());
                    if (!string.IsNullOrEmpty(user?.ProfilePicture))
                    {
                        controller.ViewData["SidebarAvatarUrl"] = user.ProfilePicture;
                        controller.ViewData["NavbarAvatarUrl"]  = user.ProfilePicture;
                    }

                    // 3. Cart count badge in navbar
                    var cart = await _orderService.GetCartAsync(userId);
                    controller.ViewData["CartCount"] = cart.Items.Count();
                }
                catch
                {
                    // Silently fail — don't break the page if this errors
                }
            }
        }

        await next();
    }
}
