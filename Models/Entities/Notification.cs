using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicOne.Models.Entities
{
    public class Notification
    {
        [Key]
        public int NotificationID { get; set; }

        [Required]
        public int ScheduleID { get; set; }

        [Required]
        [MaxLength(20)]
        public string PatientNIC { get; set; }

        [Required]
        [MaxLength(500)]
        public string Message { get; set; }

        [Required]
        public DateTime SentDate { get; set; }

        [Required]
        public bool IsRead { get; set; }


        [ForeignKey("ScheduleID")]
        public ClinicSchedule ClinicSchedule { get; set; }
        [ForeignKey("PatientNIC")]
        public Patient Patient { get; set; }
    }
}
