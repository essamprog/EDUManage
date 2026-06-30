// Certificate.cs
namespace EduManage.Core.Entities;

public class Certificate : BaseEntity
{
    public int EnrollmentId { get; set; }
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public string CertificateUrl { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Enrollment Enrollment { get; set; } = null!;
    public ApplicationUser Student { get; set; } = null!;
    public Course Course { get; set; } = null!;
}