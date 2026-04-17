namespace ClinicOne.Models.ViewModels.Pharmacist
{
    public class ExternalPrescriptionPdfModel
    {
        public string PatientName { get; set; } = string.Empty;
        public string NIC { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public List<MedicineItemViewModel> Medicines { get; set; } = new();
    }
}
