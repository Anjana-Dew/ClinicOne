namespace ClinicOne.Models.ViewModels.Pharmacist
{
    public class PrescriptionVM
    {
        public string PatientName { get; set; } = "";
        public string PatientNIC { get; set; } = "";
        public int PrescriptionID { get; set; }
        public List<MedicineVM> Medicines { get; set; } = new();
    }
}
