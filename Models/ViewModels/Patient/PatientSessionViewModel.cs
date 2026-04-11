using System;

namespace ClinicOne.Models.ViewModels.Patient
{
    public class PatientSessionViewModel
    {
        public int ScheduleID { get; set; }
        public string SessionName { get; set; }
        public DateTime ClinicDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsNextSession { get; set; }
    }
}