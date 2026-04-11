using ClinicOne.Models.Entities;

namespace ClinicOne.Models.ViewModels.Patient
{
    public class PatientClinicSchedule
    {
        public int ScheduleID { get; set; }
        public string PatientNIC { get; set; }
        public int SessionID { get; set; }
        public DateTime ClinicDate { get; set; }
        public DateTime AssignedDate { get; set; }

    
        public ClinicSession Session { get; set; }
    
}
}
