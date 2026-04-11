using ClinicOne.Models.Entities;

namespace ClinicOne.Models.ViewModels.Pharmacist
{
    public class MedicineItemViewModel
    {
        public int PrescMedID { get; set; }
        public int PrescriptionID { get; set; }

        // Make strings nullable
        public string? MedicineName { get; set; }
        public string? Status { get; set; }
        public string? Reason { get; set; }
        public string? Dosage { get; set; }
        public string? Duration { get; set; }

        public bool PatientConfirmed { get; set; }
        public int TimesPerDay { get; set; }
    }
}
