using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicOne.Models.Entities
{
    public class ClinicSchedule
    {
        [Key]
        public int ScheduleID { get; set; }

        [Required]
        [MaxLength(20)]
        public string PatientNIC { get; set; }

        [Required]
        public int SessionID { get; set; }

        [Required]
        public DateTime ClinicDate { get; set; }

        [Required]
        public DateTime AssignedDate { get; set; }


        [ForeignKey("PatientNIC")]
        public Patient Patient { get; set; }

        [ForeignKey("SessionID")]
        public ClinicSession ClinicSession { get; set; }

        public ICollection<Notification> Notifications { get; set; }
    }
}
