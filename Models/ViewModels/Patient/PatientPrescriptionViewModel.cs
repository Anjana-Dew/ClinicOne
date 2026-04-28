namespace ClinicOne.Models.ViewModels.Patient
{
    public class PatientPrescriptionViewModel
    {
        public int PrescriptionID { get; set; }
        public DateTime PrescriptionDate { get; set; }
        public string? Notes { get; set; }

        public string? PDFPath { get; set; }
        public bool HasExternalPrescription { get; set; }

        public bool PharmacyPending { get; set; }

        public bool AllGiven { get; set; }

        public bool ShowConfirmButton { get; set; }

        public bool ShowCompleted { get; set; }

        public bool IsPast { get; set; }
        public bool IsActive { get; set; }

        public List<PatientPrescriptionMedicine> Medicines { get; set; } = new();
    }

    public class PatientPrescriptionMedicine
    {
        public string? MedicineName { get; set; }
        public string? Dosage { get; set; }
        public int TimesPerDay { get; set; }
        public string? Duration { get; set; }
        public string? Status { get; set; }  // null = pharmacist hasn't acted yet
        public string? Reason { get; set; }  // reason shown if Not Given
        public bool PatientConfirmed { get; set; }
    }
}