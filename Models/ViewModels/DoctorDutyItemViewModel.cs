namespace ClinicOne.Models.ViewModels
{
    public class DoctorDutyItemViewModel
    {
        public int DoctorID { get; set; }
        public int SessionID { get; set; }
        public DateTime ClinicDate { get; set; }
        public string DoctorName { get; set; }
    }
}
