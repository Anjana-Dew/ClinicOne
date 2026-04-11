namespace ClinicOne.Models.ViewModels.Doctor
{
    public class PatientProgressViewModel
    {
        public string PatientNIC { get; set; }
        public DateTime ProgressDate { get; set; }
        public string SuggestedStatus { get; set; }
        public string CurrentStatus { get; set; }
        public bool IsConfirmed { get; set; }
        public string? DoctorNotes { get; set; }
    }
}
