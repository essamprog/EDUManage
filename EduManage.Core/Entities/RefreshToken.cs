// RefreshToken.cs
namespace EduManage.Core.Entities;

public class RefreshToken : BaseEntity
{
    public int UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }

    // Navigation
    public ApplicationUser User { get; set; } = null!;
}