// Notification.cs
using EduManage.Core.Enums;

namespace EduManage.Core.Entities;

public class Notification : BaseEntity
{
    public int UserId { get; set; }
    public NotificationType Type { get; set; } = NotificationType.System;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Link { get; set; }
    public bool IsRead { get; set; } = false;

    // Navigation
    public ApplicationUser User { get; set; } = null!;
}