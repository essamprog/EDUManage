// Course.cs
using EduManage.Core.Enums;

namespace EduManage.Core.Entities;

public class Course : BaseEntity
{
    public int InstructorId { get; set; }
    public int? CategoryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string? Description { get; set; }
    public CourseLevel Level { get; set; } = CourseLevel.Beginner;
    public CourseStatus Status { get; set; } = CourseStatus.Draft;
    public decimal Price { get; set; } = 0;
    public decimal? OriginalPrice { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? ThumbnailKey { get; set; }
    public string? PromoVideoUrl { get; set; }
    public string? PromoVideoKey { get; set; }
    public int TotalDuration { get; set; } = 0;   // بالثواني
    public int TotalLessons { get; set; } = 0;
    public int TotalStudents { get; set; } = 0;
    public decimal AverageRating { get; set; } = 0;
    public int ReviewCount { get; set; } = 0;
    public bool IsBestseller { get; set; } = false;
    public string Language { get; set; } = "Arabic";
    public DateTime? PublishedAt { get; set; }

    // Navigation
    public InstructorProfile Instructor { get; set; } = null!;
    public Category? Category { get; set; }
    public ICollection<CourseSection> Sections { get; set; } = [];
    public ICollection<Enrollment> Enrollments { get; set; } = [];
    public ICollection<Review> Reviews { get; set; } = [];
    public ICollection<OrderItem> OrderItems { get; set; } = [];
    public ICollection<CartItem> CartItems { get; set; } = [];
    public ICollection<Wishlist> Wishlists { get; set; } = [];
    public ICollection<Certificate> Certificates { get; set; } = [];
}