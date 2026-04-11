namespace ClinicOne.Models.ViewModels.Doctor
{
    public class ScheduleClinicViewModel
    {
        public string PatientNIC { get; set; }
        public DateTime ClinicDate { get; set; }
        public int SelectedSessionID { get; set; }
        public List<ClinicSessionItemViewModel> Sessions { get; set; }
    }
}
