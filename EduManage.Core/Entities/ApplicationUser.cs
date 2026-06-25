// ApplicationUser.cs
using EduManage.Core.Enums;
using Microsoft.AspNetCore.Identity;

namespace EduManage.Core.Entities;

public class ApplicationUser : IdentityUser<int>
{
    // ─────────── IdentityUser ──────────────
    public string FullName { get; set; } = string.Empty;

    public string? ProfilePicture { get; set; }
    public string? ProfilePictureKey { get; set; }   // Cloudinary public key للحذف

    public string? Bio       { get; set; }
    public string? Website   { get; set; }
    public string? Linkedin  { get; set; }
    public string? Github    { get; set; }

    public UserStatus Status { get; set; } = UserStatus.Active;

    // Soft Delete
    public DateTime  CreatedAt  { get; set; } = DateTime.UtcNow;
    public DateTime  UpdatedAt  { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt  { get; set; }

    // ── Navigation Properties ──────────────────────────────
    public InstructorProfile? InstructorProfile { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
    public ICollection<Order> Orders { get; set; } = [];
    public ICollection<Enrollment> Enrollments { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
    public ICollection<Notification> Notifications { get; set; } = [];
    public ICollection<InstructorApplication> Applications { get; set; } = [];
}