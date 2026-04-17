using ClinicOne.Models.Entities;

namespace ClinicOne.Models.ViewModels.Pharmacist
{
    public class MedicineItemViewModel
    {
        public int PrescMedID { get; set; }
        public int PrescriptionID { get; set; }

        public string? MedicineName { get; set; }
        public string? Status { get; set; }   // Given / Not Given / Partially Given
        public string? Reason { get; set; }

        public string? Dosage { get; set; }
        public string? Duration { get; set; }

        public int TimesPerDay { get; set; }

        public bool PatientConfirmed { get; set; }
    }
}
