using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicOne.Models.Entities
{
    public class ExternalPrescription
    {
        [Key]
        public int ExternalPresID { get; set; }

        [Required]
        public DateTime GeneratedDate { get; set; }

        [Required]
        public int PrescriptionID { get; set; }

        [Required]
        [MaxLength(255)]
        public string PDFPath { get; set; }


        [ForeignKey("PrescriptionID")]
        public Prescription Prescription { get; set; }
    }
}
