using System.ComponentModel.DataAnnotations;

namespace ClinicOne.Models.ViewModels.Admin
{
    public class RegisterAdminViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }
    }
}
