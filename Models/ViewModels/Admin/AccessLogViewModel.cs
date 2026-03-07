namespace ClinicOne.Models.ViewModels.Admin
{
    public class AccessLogViewModel
    {
        public DateTime AccessDateTime { get; set; }
        public int DoctorID { get; set; }
        public string DoctorName { get; set; }
        public string RegistrationNumber { get; set; }
        public string PatientName { get; set; }
        public string PatientNIC { get; set; }
        public string AccessAction {  get; set; }
    }
}
