using ClinicOne.Data;
using ClinicOne.Models.ViewModels.Pharmacist;
using ClinicOne.Services;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;

namespace ClinicOne.Areas.Pharmacist.Controllers
{
    [Area("Pharmacist")]
    public class PrescriptionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ExternalPrescriptionService _externalService;

        public PrescriptionController(
            ApplicationDbContext context,
            ExternalPrescriptionService externalService)
        {
            _context = context;
            _externalService = externalService;
        }       

        // 🔍 SEARCH
        [HttpGet]
        public IActionResult Search(string nic)
        {
            if (string.IsNullOrWhiteSpace(nic))
                return Json(new { success = false, message = "NIC required" });

            var patient = _context.Patients.FirstOrDefault(p => p.PatientNIC == nic);

            if (patient == null)
                return Json(new { success = false, message = "Patient not found" });

            var prescription = _context.Prescriptions
                .Where(p => p.PatientNIC == nic)
                .OrderByDescending(p => p.PrescriptionDate)
                .FirstOrDefault();

            if (prescription == null)
                return Json(new { success = false, message = "No prescription found" });

            var meds = _context.PrescriptionMedicines
                .Where(m => m.PrescriptionID == prescription.PrescriptionID)
                .ToList();

            var result = meds.Select(m => new MedicineVM
            {
                PrescMedID = m.PrescMedID,
                MedicineName = m.MedicineName,
                Dosage = m.Dosage,
                Duration = m.Duration,
                TimesPerDay = m.TimesPerDay,
                Status = m.Status,
                Reason = m.Reason
            }).ToList();

            return Json(new
            {
                success = true,
                patientName = patient.FullName,
                patientNIC = patient.PatientNIC,
                medicines = result
            });
        }

        // 💾 CONFIRM
        [HttpPost]
        public IActionResult Confirm([FromBody] List<ConfirmMedicineVM> data)
        {
            if (data == null || !data.Any())
                return Json(new { success = false });

            foreach (var item in data)
            {
                var med = _context.PrescriptionMedicines
                    .FirstOrDefault(x => x.PrescMedID == item.PrescMedID);

                if (med != null)
                {
                    med.Status = item.Status;
                    med.Reason = item.Reason;
                    med.PatientConfirmed = true;
                }
            }

            _context.SaveChanges();
            return Json(new { success = true });
        }

        // 📄 PDF
        [HttpPost]
        public IActionResult GenerateExternal([FromBody] ExternalPrescriptionRequest request)
        {
            if (request == null || request.Medicines == null || !request.Medicines.Any())
                return BadRequest("No medicines");

            var model = new ExternalPrescriptionPdfModel
            {
                PatientName = request.PatientName,
                NIC = request.NIC,
                Medicines = request.Medicines
            };

            var pdf = _externalService.GeneratePdf(model.Medicines);

            return File(pdf, "application/pdf", "External.pdf");
        }
    }
}