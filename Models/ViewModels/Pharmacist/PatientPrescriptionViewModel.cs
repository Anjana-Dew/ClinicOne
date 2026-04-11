namespace ClinicOne.Models.ViewModels.Pharmacist
{
    public class PatientPrescriptionViewModel
    {
        public string PatientNIC { get; set; }
        public string PatientName { get; set; }
        public int PrescriptionID { get; set; }

        public List<MedicineItemViewModel> Medicines { get; set; } = new();
    }
}
