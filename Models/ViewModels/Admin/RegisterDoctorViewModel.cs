using System.ComponentModel.DataAnnotations;

namespace ClinicOne.Models.ViewModels.Admin
{
    public class RegisterDoctorViewModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name="Registration Number")]
        [RegularExpression(@"^SLMC\d{6}$", ErrorMessage = "Registration must start with SLMC followed by 6 digits.")]
        public string RegistrationNumber { get; set; }

        [Required]
        [Display(Name= "Specialization")]
        public string Specialization {  get; set; }
    }
}
