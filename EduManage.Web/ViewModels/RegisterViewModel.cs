using System.ComponentModel.DataAnnotations;

namespace EduManage.Web.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        public string FullName { get; set; } = "";

        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        public string Password { get; set; } = "";

        [Required]
        public string ConfirmPassword { get; set; } = "";
    }
}