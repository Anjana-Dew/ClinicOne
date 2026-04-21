namespace ClinicOne.Models.ViewModels.Patient
{
    public class PatientPrescriptionViewModel
    {
        public int PrescriptionID { get; set; }
        public DateTime PrescriptionDate { get; set; }

        public string DoctorName { get; set; }
        public string DoctorID { get; set; }

        public string? PDFPath { get; set; }

        public bool IsConfirmed { get; set; }

        public int TotalMedicines { get; set; }
        public int TakenMedicines { get; set; }

        public List<PatientPrescriptionMedicine> Medicines { get; set; } = new();
    }

    public class PatientPrescriptionMedicine
    {
        public string MedicineName { get; set; }
        public string Status { get; set; }

        public string? Dosage { get; set; }
        public int TimesPerDay { get; set; }

        public bool PatientConfirmed { get; set; }
    }
}