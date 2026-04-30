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

            nic = nic?.Trim();

            var patient = _context.Patients
                .FirstOrDefault(p => p.PatientNIC != null &&
                                     p.PatientNIC.Trim() == nic);

            if (patient == null)
                return Json(new { success = false, message = "Patient not found" }); 


            var prescription = _context.Prescriptions
                .Where(p => p.PatientNIC == nic && p.IsCompleted == false)
                .OrderByDescending(p => p.PrescriptionDate)
                .FirstOrDefault();

            if (prescription == null)
                return Json(new { success = false, message = "No prescription found" });

            var medicines = _context.PrescriptionMedicines
               .Where(m =>
                   m.PrescriptionID == prescription.PrescriptionID &&
                   !(m.Status == "Given" && m.PatientConfirmed == true)
               )
               .ToList();


            if (!medicines.Any())
            {
                return Json(new
                {
                    success = true,
                    message = "All medicines already processed",
                    patientName = patient.FullName,
                    patientNIC = patient.PatientNIC,
                    prescriptionId = prescription.PrescriptionID,
                    medicines = new List<object>()
                });
            }

            var meds = medicines.Select(m => new MedicineVM
            {
                PrescMedID = m.PrescMedID,
                MedicineName = m.MedicineName ?? "-",
                Dosage = m.Dosage ?? "-",
                Duration = m.Duration ?? "-",
                TimesPerDay = m.TimesPerDay,
                Status = m.Status ?? "Not Given",
                Reason = m.Reason ?? ""
            }).ToList();

            return Json(new
            {
                success = true,
                patientName = patient.FullName ?? "",
                patientNIC = patient.PatientNIC,
                prescriptionId = prescription.PrescriptionID,
                medicines = meds
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
                .Select(m => new { m.PrescMedID, m.PrescriptionID })
                .ToList();

            int prescriptionId = 0;

            foreach (var med in meds)
            {
                var item = data.FirstOrDefault(x => x.PrescMedID == med.PrescMedID);
                if (item == null) continue;

                prescriptionId = med.PrescriptionID;

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
            }

            _context.SaveChanges();

            var patientNIC = _context.Prescriptions
                .Where(p => p.PrescriptionID == prescriptionId)
                .Select(p => p.PatientNIC)
                .FirstOrDefault();

            var confirmedMeds = _context.PrescriptionMedicines
            .Where(m => ids.Contains(m.PrescMedID))
            .ToList();

            foreach (var med in confirmedMeds)
            {
                if (med.Status == "Given")
                {
                    int days = ParseDurationToDays(med.Duration);

                    var reminder = new MedicineReminder
                    {
                        PatientNIC = patientNIC,  
                        PrescMedID = med.PrescMedID,
                        StartDate = DateTime.Today,
                        EndDate = DateTime.Today.AddDays(days),
                        IsActive = true
                    };

                    _context.MedicineReminders.Add(reminder);
                }
            }

            _context.SaveChanges();


            if (prescriptionId != 0)
            {
                var prescription = new Prescription
                {
                    PrescriptionID = prescriptionId
                };

                _context.Prescriptions.Attach(prescription);
                prescription.IsCompleted = true;

                _context.Entry(prescription)
                    .Property(p => p.IsCompleted)
                    .IsModified = true;

                _context.SaveChanges();
            }

            var medsFull = _context.PrescriptionMedicines
                .Where(m => m.PrescriptionID == prescriptionId)
                .ToList();

            _notificationService.NotifyPrescriptionProcessed(patientNIC, medsFull);


            bool hasNotGiven = data.Any(x => x.Status != "Given");

            return Json(new
            {
                success = true,
                hasNotGiven = hasNotGiven
            });
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

            _notificationService.NotifyExternalPdfReady(request.NIC);

            return File(pdfBytes, "application/pdf", fileName);
        }
        private int ParseDurationToDays(string duration)
        {
            if (string.IsNullOrEmpty(duration)) return 0;

            var parts = duration.Split(' ');
            if (parts.Length != 2) return 0;

            int value = int.Parse(parts[0]);

            if (parts[1].StartsWith("week")) return value * 7;
            if (parts[1].StartsWith("day")) return value;

            return 0;
        }

    }
}﻿
