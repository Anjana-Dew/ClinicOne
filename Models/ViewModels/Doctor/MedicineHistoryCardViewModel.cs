namespace ClinicOne.Models.ViewModels.Doctor
{
    public class MedicineHistoryCardViewModel
    {
        public DateTime PrescriptionDate { get; set; }
        public DateTime? UntilDate { get; set; }

        public List<MedicineItemViewModel> Medicines { get; set; }
    }
}
