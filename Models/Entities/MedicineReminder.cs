using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicOne.Models.Entities
{
    public class MedicineReminder
    {
        [Key]
        public int ReminderID { get; set; }

        [Required]
        [MaxLength(20)]
        public string PatientNIC { get; set; }

        [Required]
        public int PrescMedID { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public bool IsActive { get; set; }


        [ForeignKey("PatientNIC")]
        public Patient Patient { get; set; }

        [ForeignKey("PrescMedID")]
        public PrescriptionMedicine PrescriptionMedicine { get; set; }
    }
}
