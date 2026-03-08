using System.ComponentModel.DataAnnotations;

namespace ClinicOne.Models.ViewModels.Auth
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Please enter a new password.")]
        [MinLength(8, ErrorMessage = "The new password must contain at least 8 characters.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).+$",
            ErrorMessage = "Password must contain at least one uppercase letter and one number.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Please confirm your password.")]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
