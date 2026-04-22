namespace ClinicOne.Models.ViewModels.Doctor
{
    public class MedicineInputViewModel
    {
        public string MedicineName { get; set; }
        public string Dosage { get; set; }
        public int TimesPerDay { get; set; }
        public int? DurationValue { get; set; }
        public string DurationUnit { get; set; }
        public bool UntilNextVisit { get; set; }
    }
}
