namespace ClinicOne.Models.ViewModels.Doctor
{
    public class CreatePrescriptionViewModel
    {
        public string PatientNIC { get; set; }
        public string Notes { get; set; }
        public List<MedicineInputViewModel> Medicines { get; set; }

        public List<TestRowViewModel> Tests { get; set; }
        public List<TestOptionViewModel> AvailableTests { get; set; }
    }
}
