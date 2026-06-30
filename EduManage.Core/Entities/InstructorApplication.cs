// InstructorApplication.cs
using EduManage.Core.Enums;

namespace EduManage.Core.Entities;

public class InstructorApplication : BaseEntity
{
    public int UserId { get; set; }
    public string Expertise { get; set; } = string.Empty;
    public string Experience { get; set; } = string.Empty;
    public string? ResumeUrl { get; set; }
    public ApplicationStatus Status { get; set; } = ApplicationStatus.Pending;
    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ApplicationUser User { get; set; } = null!;
}