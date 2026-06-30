// CartItem.cs
namespace EduManage.Core.Entities;

public class CartItem : BaseEntity
{
    public int StudentId { get; set; }
    public int CourseId { get; set; }
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ApplicationUser Student { get; set; } = null!;
    public Course Course { get; set; } = null!;
}