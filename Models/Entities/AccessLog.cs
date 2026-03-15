using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicOne.Models.Entities
{
    public class AccessLog
    {
        [Key]
        public int LogID { get; set; }

        [Required]
        public int DoctorID { get; set; }

        [Required]
        [MaxLength(20)]
        public string PatientNIC { get; set; }

        [Required]
        [MaxLength(100)]
        public string Action {  get; set; }

        [Required]
        public DateTime AccessDateTime { get; set; }


        [ForeignKey("PatientNIC")]
        public Patient Patient { get; set; }

        [ForeignKey("DoctorID")]
        public Doctor Doctor { get; set; }
    }
}
