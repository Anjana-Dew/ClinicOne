namespace ClinicOne.Models.ViewModels
{
    public class SaveDoctorDutyRequestViewModel
    {
        public int SessionId { get; set; }
        public DateTime ClinicDate { get; set; }
        public List<int> DoctorIds { get; set; }
    }
}
