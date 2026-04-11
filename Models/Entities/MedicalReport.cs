using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicOne.Models.Entities
{
    public class MedicalReport
    {
        [Key]
        public int ReportID { get; set; }

        [Required]
        [MaxLength(20)]
        public string PatientNIC { get; set; }

        [Required]
        public DateTime ReportDate { get; set; }

        [Required]
        public DateTime UploadedDate { get; set; }

        [Required]
        [MaxLength(255)]
        public string ReportPath { get; set; }


        [ForeignKey("PatientNIC")]
        public Patient Patient { get; set; }

        public ICollection<ReportTestResult> ReportTestResults { get; set; }
    }
}
