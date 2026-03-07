namespace ClinicOne.Models.ViewModels.Admin
{
    public class RemoveDoctorDutyRequestViewModel
    {
        public int DoctorID {  get; set; }
        public int SessionID { get; set; }
        public DateTime ClinicDate { get; set; }
    }
}
