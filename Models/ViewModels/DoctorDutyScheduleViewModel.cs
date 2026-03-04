namespace ClinicOne.Models.ViewModels
{
    public class DoctorDutyScheduleViewModel
    {
        public List<DoctorSelectViewModel> Doctors { get; set; }
        public List<ClinicSessionSelectViewModel> Sessions { get; set; }
    }
}
