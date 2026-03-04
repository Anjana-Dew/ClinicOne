using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicOne.Models.Entities
{
    public class DoctorDutySchedule
    {
        [Key]
        public int DutyID { get; set; }

        [Required]
        public int DoctorID { get; set; }

        [Required]
        public int SessionID { get; set; }

        [Required]
        public DateTime ClinicDate { get; set; }


        [ForeignKey("DoctorID")]
        public Doctor Doctor { get; set; }

        [ForeignKey("SessionID")]
        public ClinicSession ClinicSession { get; set; }
    }
}
