using EduManage.Application.DTOs.System;
using EduManage.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EduManage.Application.Interfaces;

public interface INotificationService
{
    Task SendAsync(int userId, NotificationType type, string title, string message, string? link = null);
    Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(int userId);
    Task MarkAsReadAsync(int notificationId);
    Task MarkAllAsReadAsync(int userId);
}