using System.ComponentModel.DataAnnotations;

namespace ClinicOne.Models.ViewModels.Admin
{
    public class RegisterAdminViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
