// ApplicationUser.cs
namespace EduManage.Core.Entities
{
    public class RefreshToken
    {
        public int UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;
    }
}