using System.ComponentModel.DataAnnotations;


namespace ClinicOne.Models.Entities
{
    public class ClinicSession
    {
        [Key]
        public int SessionID { get; set; }

        [Required]
        [MaxLength(100)]
        public string SessionName { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [Required]
        public int MaxSlots { get; set; }

        public ICollection<ClinicSchedule> ClinicSchedules { get; set; }
        public ICollection<DoctorDutySchedule> DoctorDutySchedules { get; set; }
    }
}
