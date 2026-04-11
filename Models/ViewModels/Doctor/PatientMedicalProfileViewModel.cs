namespace ClinicOne.Models.ViewModels.Doctor
{
    public class PatientMedicalProfileViewModel
    {
        public string PatientNIC { get; set; }
        public string FullName { get; set; }
        public string BloodType { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public decimal?  Height { get; set; }
        public decimal? Weight { get; set; }
        public string BloodPressure { get; set; }
        public decimal? BMI { get; set; }
        public PatientProgressViewModel Progress { get; set; }

        public List<ClinicSessionItemViewModel> ClinicSessions { get; set; }
        public List<MedicalReportCardViewModel> MedicalReports { get; set; }
        public List<MedicineHistoryCardViewModel> MedicineHistories { get; set; }
    }
}
