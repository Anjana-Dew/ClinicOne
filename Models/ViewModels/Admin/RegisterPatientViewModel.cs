using System.ComponentModel.DataAnnotations;

namespace ClinicOne.Models.ViewModels.Admin
{
    public class RegisterPatientViewModel
    {
        [Required]
        public string FullName { get; set; }
            
        [Required]
        public string NIC {  get; set; }

        [Required (ErrorMessage = "The Birth Date is required")]
        [DataType(DataType.Date)]
        public DateTime? DOB { get; set; }

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

        //[Range(50, 250, ErrorMessage = "Invalid systolic Value")]
        //public int? Systolic { get; set; }
        //[Range(30, 150, ErrorMessage = "Invalid diastolic Value")]
        //public int? Diastolic { get; set; }

    }
}
