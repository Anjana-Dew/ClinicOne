namespace ClinicOne.Models.ViewModels
{
    public class RemoveDoctorDutyRequestViewModel
    {
        public int DoctorID {  get; set; }
        public int SessionID { get; set; }
        public DateTime ClinicDate { get; set; }
    }
}
