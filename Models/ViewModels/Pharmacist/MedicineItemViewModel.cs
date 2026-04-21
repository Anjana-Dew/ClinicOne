using ClinicOne.Models.Entities;

namespace ClinicOne.Models.ViewModels.Pharmacist
{
    public class MedicineItemViewModel
    {
        public string MedicineName { get; set; } = "-";
        public string Dosage { get; set; } = "-";
        public string Duration { get; set; } = "-";
        public string Status { get; set; } = "Unknown";
        public string Reason { get; set; } = "-";
        public int TimesPerDay { get; set; }
    }
}
