using ClinicOne.Models.Entities;

namespace ClinicOne.Models.ViewModels.Pharmacist
{
    public class PatientPrescriptionViewModel
    {
        public string PatientNIC { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;

        public int PrescriptionID { get; set; }

        public List<MedicineItemViewModel> Medicines { get; set; } = new();

        public ICollection<PrescriptionMedicine> PrescriptionMedicines { get; set; }
    }
}

