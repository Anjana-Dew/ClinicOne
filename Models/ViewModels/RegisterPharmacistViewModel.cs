using System.ComponentModel.DataAnnotations;

namespace ClinicOne.Models.ViewModels
{
    public class RegisterPharmacistViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Registration Number")]
        [RegularExpression(@"^NMRA\d{7}$", ErrorMessage = "Registration Number must start with NMRA followed by 7 digits.")]
        public string RegistrationNumber { get; set; }
    }
}
