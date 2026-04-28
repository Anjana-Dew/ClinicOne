using ClinicOne.Data;
using ClinicOne.Models.Entities;
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
        private readonly NotificationService _notificationService;

        public PrescriptionController(
            ApplicationDbContext context,
            NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        // SEARCH
        [HttpGet]
        public IActionResult Search(string nic)
        {
            if (string.IsNullOrWhiteSpace(nic))
                return Json(new { success = false, message = "NIC required" });

            var patient = _context.Patients
                .FirstOrDefault(p => p.PatientNIC == nic);

            if (patient == null)
                return Json(new { success = false, message = "Patient not found" });

            var prescription = _context.Prescriptions
    .Where(p => p.PatientNIC == nic)
    .Where(p => _context.PrescriptionMedicines
        .Any(m => m.PrescriptionID == p.PrescriptionID && !m.PatientConfirmed))
    .OrderByDescending(p => p.PrescriptionDate)
    .FirstOrDefault();

            if (prescription == null)
                return Json(new { success = false, message = "No prescription found" });

            var medsRaw = _context.PrescriptionMedicines
            .Where(m => m.PrescriptionID == prescription.PrescriptionID)
            .Select(m => new
            {
        m.PrescMedID,

        MedicineName = m.MedicineName == null ? "-" : m.MedicineName,
        Dosage = m.Dosage == null ? "-" : m.Dosage,
        Duration = m.Duration == null ? "-" : m.Duration,

        TimesPerDay = m.TimesPerDay,

        Status = m.Status == null ? "Not Given" : m.Status,
        Reason = m.Reason == null ? "" : m.Reason
    })
    .ToList();

            var meds = medsRaw.Select(m => new MedicineVM
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
                patientName = patient.FullName ?? "",
                patientNIC = patient.PatientNIC,
                prescriptionId = prescription.PrescriptionID,

                medicines = meds.Select(m => new
                {
                    prescMedID = m.PrescMedID,
                    medicineName = m.MedicineName,
                    dosage = m.Dosage,
                    duration = m.Duration,
                    timesPerDay = m.TimesPerDay,
                    status = m.Status,
                    reason = m.Reason
                })
            });
        }


        //SAVE
        [HttpPost]
        public IActionResult Confirm([FromBody] List<ConfirmMedicineVM> data)
        {
            if (data == null || !data.Any())
                return BadRequest("No data");

            var ids = data.Select(x => x.PrescMedID).ToList();

            var meds = _context.PrescriptionMedicines
                .Where(m => ids.Contains(m.PrescMedID))
                .Select(m => new { m.PrescMedID })
                .ToList();

            foreach (var med in meds)
            {
                var item = data.FirstOrDefault(x => x.PrescMedID == med.PrescMedID);
                if (item == null) continue;

                var entity = new PrescriptionMedicine
                {
                    PrescMedID = med.PrescMedID
                };

                _context.PrescriptionMedicines.Attach(entity);

                entity.Status = string.IsNullOrEmpty(item.Status)
                    ? "Not Given"
                    : item.Status;

                entity.Reason = entity.Status == "Given"
                    ? null
                    : (string.IsNullOrWhiteSpace(item.Reason)
                        ? "Not specified"
                        : item.Reason);

                entity.PatientConfirmed = entity.Status == "Given";

                _context.Entry(entity).Property(x => x.Status).IsModified = true;
                _context.Entry(entity).Property(x => x.Reason).IsModified = true;
                _context.Entry(entity).Property(x => x.PatientConfirmed).IsModified = true;
            }

            _context.SaveChanges();

            return Json(new { success = true });
        }

        //PDF
        [HttpPost]
        public IActionResult GenerateExternal([FromBody] ExternalPrescriptionRequest request)
        {
            if (request == null || request.Medicines == null || !request.Medicines.Any())
                return BadRequest("Invalid request");

            var prescription = _context.Prescriptions
                .Where(p => p.PatientNIC == request.NIC)
                .OrderByDescending(p => p.PrescriptionDate)
                .FirstOrDefault();

            if (prescription == null)
                return BadRequest("No valid prescription found");

            var doc = new ExternalPrescriptionDocument(new ExternalPrescriptionPdfModel
            {
                PatientName = request.PatientName,
                NIC = request.NIC,
                Medicines = request.Medicines
            });

            byte[] pdfBytes = doc.GeneratePdf();

            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "external-prescriptions");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            string fileName = $"External_{DateTime.Now.Ticks}.pdf";
            string fullPath = Path.Combine(folderPath, fileName);

            System.IO.File.WriteAllBytes(fullPath, pdfBytes);

            var external = new ExternalPrescription
            {
                PrescriptionID = prescription.PrescriptionID,
                PDFPath = "/external-prescriptions/" + fileName,
                GeneratedDate = DateTime.Now
            };

            _context.ExternalPrescriptions.Add(external);
            _context.SaveChanges();

            // Notify the patient
            _notificationService.NotifyExternalPdfReady(request.NIC);

            return File(pdfBytes, "application/pdf", fileName);
        }

    }
}
