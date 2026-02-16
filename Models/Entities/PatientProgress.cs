using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicOne.Models.Entities
{
    public class PatientProgress
    {
        [Key]
        public int progressID { get; set; }

        [Required]
        [MaxLength(20)]
        public string PatientNIC { get; set; }

        [Required]
        public int ReportID { get; set; }

        [Required]
        [MaxLength(20)]
        public string ProgressStatus { get; set; }

        [Required]
        public bool IsConfirmed { get; set; }

        [MaxLength(500)]
        public string DoctorNotes { get; set; }
        [Required]
        public DateTime RecordedDate { get; set; }


        [ForeignKey("PatientNIC")]
        public Patient Patient { get; set; }

        [ForeignKey("ReportID")]
        public MedicalReport MedicalReport { get; set; }
    }
}
