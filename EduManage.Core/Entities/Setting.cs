// Setting.cs
namespace EduManage.Core.Entities;

public class Setting : BaseEntity
{
    public string SettingKey { get; set; } = string.Empty;
    public string? SettingValue { get; set; }
}