// AuditLog.cs
namespace EduManage.Core.Entities;

public class AuditLog : BaseEntity
{
    public int AdminId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? EntityType { get; set; }
    public int? EntityId { get; set; }
    public string? Details { get; set; }   // JSON string

    // Navigation
    public ApplicationUser Admin { get; set; } = null!;
}