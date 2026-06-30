// Enrollment.cs
using EduManage.Core.Enums;

namespace EduManage.Core.Entities;

public class Enrollment : BaseEntity
{
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public byte Progress { get; set; } = 0;   // 0-100
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.InProgress;
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAt { get; set; }

    // Navigation
    public ApplicationUser User { get; set; } = null!;
    public Course Course { get; set; } = null!;
    public Certificate? Certificate { get; set; }
}