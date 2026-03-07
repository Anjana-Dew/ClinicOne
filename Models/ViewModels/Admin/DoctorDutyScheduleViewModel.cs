namespace ClinicOne.Models.ViewModels.Admin
{
    public class DoctorDutyScheduleViewModel
    {
        public List<DoctorSelectViewModel> Doctors { get; set; }
        public List<ClinicSessionSelectViewModel> Sessions { get; set; }

        public List<DoctorDutyItemViewModel> ExistingSchedules { get; set; }
    }
}
