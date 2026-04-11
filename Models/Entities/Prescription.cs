using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicOne.Models.Entities
{
    public class Prescription
    {
        [Key]
        public int PrescriptionID { get; set; }

        [Required]
        [MaxLength(20)]
        public string PatientNIC { get; set; }

        [Required]
        public DateTime PrescriptionDate { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [ForeignKey("PatientNIC")]
        public Patient Patient { get; set; }

        public ICollection<PrescriptionMedicine> PrescriptionMedicines { get; set; }
        public ICollection<PrescribedTest> PrescribedTests { get; set; }
        public ExternalPrescription ExternalPrescription { get; set; }
    }
}
