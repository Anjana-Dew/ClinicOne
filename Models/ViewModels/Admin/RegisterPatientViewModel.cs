using System.ComponentModel.DataAnnotations;

namespace ClinicOne.Models.ViewModels.Admin
{
    public class RegisterPatientViewModel
    {
        [Required]
        public string FullName { get; set; }
            
        [Required]
        public string NIC {  get; set; }

        [Required]
        public DateTime DOB { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        public decimal? Height { get; set; }
        public decimal? Weight { get; set; }
        public string? BloodType { get; set; }
        public string? BloodPressure { get; set; }
    }
}
