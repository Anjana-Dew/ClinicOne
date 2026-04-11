using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicOne.Models.Entities
{
    public class Patient
    {
        [Key]
        [MaxLength(20)]
        public string PatientNIC { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; }

        [Required]
        [MaxLength(200)]
        public string Address { get; set; }

        [Required]
        [MaxLength(15)]
        public string PhoneNumber { get; set; }

        public string? BloodType { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public DateTime DOB { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        public int UserAccountID { get; set; }

        [ForeignKey("UserAccountID")]
        public UserAccount UserAccount { get; set; }

        public ICollection<MedicalReport> MedicalReports { get; set; }
        public ICollection<Prescription> Prescriptions { get; set; }
        public ICollection<ClinicSchedule> ClinicSchedules { get; set; }
        public ICollection<Notification> Notifications { get; set; }
        public ICollection<PatientProgress> PatientProgresses { get; set; }
        public ICollection<MedicineReminder> MedicineReminders { get; set; }
        public ICollection<AccessLog> AccessLogs { get; set; }
        public ICollection<PatientVital> Vitals { get; set; }

    }
}
