using System.ComponentModel.DataAnnotations;

namespace EduManage.Application.DTOs.Auth;

public class ProfileDto
{
    [Required(ErrorMessage = "Full name is required")]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Bio { get; set; }

    [MaxLength(200)]
    [Url(ErrorMessage = "Please enter a valid URL")]
    public string? Website { get; set; }

    [MaxLength(200)]
    public string? Linkedin { get; set; }

    [MaxLength(200)]
    public string? Github { get; set; }
}
