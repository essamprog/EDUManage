// InstructorProfile.cs
namespace EduManage.Core.Entities;

public class InstructorProfile
{
    public int UserId { get; set; }
    public string? Bio { get; set; }
    public string? Expertise { get; set; }
    public string? Experience { get; set; }
    public decimal AverageRating { get; set; } = 0;
    public int TotalStudents { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ApplicationUser User { get; set; } = null!;
    public Wallet? Wallet { get; set; }
    public ICollection<Course> Courses { get; set; } = [];
    public ICollection<Withdrawal> Withdrawals { get; set; } = [];
}