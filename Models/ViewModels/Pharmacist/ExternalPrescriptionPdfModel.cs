namespace ClinicOne.Models.ViewModels.Pharmacist
{
    public class ExternalPrescriptionPdfModel
    {
        public string PatientName { get; set; } = "";
        public string NIC { get; set; } = "";
        public string Notes { get; set; } = "";
        public List<MedicineItemViewModel> Medicines { get; set; } = new();
    }
}
