namespace ClinicOne.Models.ViewModels.Doctor
{
    public class MedicineHistoryPageViewModel
    {
        public string PatientNIC { get; set; }
        public int? SelectedMonth {  get; set; }
        public int? SelectedYear { get; set; }
        public List<int> AvailableYears { get; set; }
        public List<MedicineHistoryCardViewModel> MedicineHistories { get; set; } 
    }
}
