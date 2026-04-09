using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicOne.Models.Entities
{
    public class PatientVital
    {
        [Key]
        public int VitalId { get; set; }
        [Required]
        public string PatientNIC { get; set; }
        public DateTime RecordedDate { get; set; } = DateTime.Now;
        public Decimal? Height { get; set; }
        public Decimal? Weight { get; set; }
        public int? Systolic {  get; set; }
        public int? Diastolic { get; set; }

        [ForeignKey("PatientNIC")]
        public Patient Patient { get; set; }

    }
}
