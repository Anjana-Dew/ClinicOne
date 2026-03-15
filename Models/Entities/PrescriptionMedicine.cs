using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicOne.Models.Entities
{
    public class PrescriptionMedicine
    {
        [Key]
        public int PrescMedID { get; set; }

        [Required]
        public int PrescriptionID { get; set; }

        [Required]
        [MaxLength(150)]
        public string MedicineName { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; }

        [MaxLength(200)]
        public string Reason { get; set; }

        [MaxLength(100)]
        public string Dosage { get; set; }

        [MaxLength(50)]
        public string Duration { get; set; }

        [Required]
        public bool PatientConfirmed { get; set; }

        public int TimesPerDay { get; set; }

        [ForeignKey("PrescriptionID")]
        public Prescription Prescription { get; set; }
        public ICollection<MedicineReminder> MedicineReminders { get; set; }
    }
}
