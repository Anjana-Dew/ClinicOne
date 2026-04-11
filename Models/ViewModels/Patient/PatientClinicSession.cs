using ClinicOne.Models.Entities;

namespace ClinicOne.Models.ViewModels.Patient
{
    public class PatientClinicSession
    {
        public int SessionID { get; set; }
        public string SessionName { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int MaxSlots { get; set; }

       
        public ICollection<ClinicSchedule> Schedules { get; set; } = new List<ClinicSchedule>();
    }
}

