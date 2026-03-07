namespace ClinicOne.Models.ViewModels.Admin
{
    public class SaveDoctorDutyRequestViewModel
    {
        public int SessionId { get; set; }
        public DateTime ClinicDate { get; set; }
        public List<int> DoctorIds { get; set; }
    }
}
