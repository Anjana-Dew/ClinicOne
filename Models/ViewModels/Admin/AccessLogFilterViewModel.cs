namespace ClinicOne.Models.ViewModels.Admin
{
    public class AccessLogFilterViewModel
    {
        public int? Month { get; set; }
        public int? Year { get; set; }
        public int? DoctorID { get; set; }
        public string AccessAction { get; set; }
        public string PatientNIC { get; set; }

    }
}
