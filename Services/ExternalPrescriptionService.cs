using ClinicOne.Models.ViewModels.Pharmacist;
using QuestPDF.Fluent;

namespace ClinicOne.Services
{
    public class ExternalPrescriptionService
    {
        public byte[] GeneratePdf(List<MedicineVM> medicines)
        {
            var model = new ExternalPrescriptionPdfModel
            {
                PatientName = "External Patient",
                NIC = "N/A",
                Medicines = medicines
            };

            var doc = new ExternalPrescriptionDocument(model);
            return doc.GeneratePdf();
        }
    }
}