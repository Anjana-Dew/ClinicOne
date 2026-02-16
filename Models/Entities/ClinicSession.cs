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
        public int MaxStlots { get; set; }

        public ICollection<ClinicSchedule> ClinicSchedules { get; set; }
        public ICollection<DoctorSchedule> DoctorSchedules { get; set; }
    }
}
