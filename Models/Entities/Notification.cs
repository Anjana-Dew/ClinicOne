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

        // Format: [Type|yyyy-MM-dd] Human readable message
        // e.g.  : [Prescription|2026-05-20] New prescription issued...
        // e.g.  : [Pharmacy] Your prescription has been processed...
        // e.g.  : [Appointment|2026-05-15] Your clinic is scheduled...
        // e.g.  : [Report|2026-04-28] A new medical report has been uploaded...
        // e.g.  : [Note] Your doctor added a progress note...
        [Required]
        [MaxLength(500)]
        public string Message { get; set; }

        [Required]
        public DateTime SentDate { get; set; } = DateTime.Now;

        [Required]
        public bool IsRead { get; set; } = false;

        [ForeignKey("ScheduleID")]
        public ClinicSchedule ClinicSchedule { get; set; }

        [ForeignKey("PatientNIC")]
        public Patient Patient { get; set; }
    }
}
