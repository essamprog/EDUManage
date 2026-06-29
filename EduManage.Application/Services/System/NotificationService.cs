// Application/Services/System/NotificationService.cs
using AutoMapper;
using EduManage.Application.DTOs.System;
using EduManage.Application.Interfaces;
using EduManage.Core.Entities;
using EduManage.Core.Enums;
using EduManage.Core.Interfaces;

namespace EduManage.Application.Services.System;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public NotificationService(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task SendAsync(
        int userId,
        NotificationType type,
        string title,
        string message,
        string? link = null)
    {
        await _uow.Notifications.AddAsync(new Notification
        {
            UserId = userId,
            Type = type,
            Title = title,
            Message = message,
            Link = link,
            IsRead = false,
            CreatedAt = DateTime.UtcNow,
        });
        await _uow.SaveChangesAsync();
    }

    public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(int userId)
    {
        var notifications = await _uow.Notifications
            .FindAsync(n => n.UserId == userId);

        return _mapper.Map<IEnumerable<NotificationDto>>(
            notifications.OrderByDescending(n => n.CreatedAt).Take(20));
    }

    public async Task MarkAsReadAsync(int notificationId)
    {
        var notification = await _uow.Notifications.GetByIdAsync(notificationId);
        if (notification is null) return;

        notification.IsRead = true;
        _uow.Notifications.Update(notification);
        await _uow.SaveChangesAsync();
    }

    public async Task MarkAllAsReadAsync(int userId)
    {
        var notifications = await _uow.Notifications
            .FindAsync(n => n.UserId == userId && !n.IsRead);

        foreach (var n in notifications)
        {
            n.IsRead = true;
            _uow.Notifications.Update(n);
        }
        await _uow.SaveChangesAsync();
    }
}