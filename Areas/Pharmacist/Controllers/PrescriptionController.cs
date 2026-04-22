using ClinicOne.Data;
using ClinicOne.Models.ViewModels.Pharmacist;
<<<<<<< HEAD
=======
using ClinicOne.Models.ViewModels.Pharmacist;
>>>>>>> main
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
<<<<<<< HEAD
            _externalService = externalService;
        }       

        // 🔍 SEARCH
=======
        }

        // 🔍 SEARCH NIC
>>>>>>> main
        [HttpGet]
        public IActionResult Search(string nic)
        {
            if (string.IsNullOrWhiteSpace(nic))
<<<<<<< HEAD
                return Json(new { success = false, message = "NIC required" });
=======
            {
                return Json(new { success = false, message = "NIC is required" });
            }
>>>>>>> main

            var patient = _context.Patients.FirstOrDefault(p => p.PatientNIC == nic);

            if (patient == null)
            {
                return Json(new { success = false, message = "Patient not found for this NIC" });
            }

            var prescription = _context.Prescriptions
                .Where(p => p.PatientNIC == nic)
                .OrderByDescending(p => p.PrescriptionDate)
                .FirstOrDefault();

            if (prescription == null)
                return Json(new { success = false, message = "No prescription found" });

<<<<<<< HEAD
            var meds = _context.PrescriptionMedicines
                .Where(m => m.PrescriptionID == prescription.PrescriptionID)
                .ToList();
=======
            // 🔥 STEP 1: LOAD DATA FROM DB FIRST (NO SELECT)
            var rawMeds = _context.PrescriptionMedicines
                .Where(m => m.PrescriptionID == prescription.PrescriptionID)
                .ToList();   // 🔥 THIS LINE FIXES YOUR ERROR

            // 🔥 STEP 2: SAFE MAPPING (AFTER DB LOAD)
            var medicines = rawMeds.Select(m => new MedicineVM
            {
                PrescMedID = m.PrescMedID,
                MedicineName = m.MedicineName ?? "-",
                Dosage = m.Dosage ?? "-",
                Duration = m.Duration ?? "-",
                TimesPerDay = m.TimesPerDay,
                Status = m.Status ?? "Not Given",
                Reason = m.Reason ?? ""
            }).ToList();
>>>>>>> main

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
<<<<<<< HEAD
                patientName = patient.FullName,
                patientNIC = patient.PatientNIC,
                medicines = result
            });
        }

        // 💾 CONFIRM
=======
                patientName = patient.FullName ?? "",
                patientNIC = patient.PatientNIC,
                prescriptionID = prescription.PrescriptionID,
                medicines
            });
        }

        // 💾 SAVE STATUS
>>>>>>> main
        [HttpPost]
        public IActionResult Confirm([FromBody] List<ConfirmMedicineVM> data)
        {
            if (data == null || !data.Any())
<<<<<<< HEAD
                return Json(new { success = false });

            foreach (var item in data)
            {
                var med = _context.PrescriptionMedicines
                    .FirstOrDefault(x => x.PrescMedID == item.PrescMedID);
=======
                return BadRequest("No data received");

            foreach (var item in data)
            {
                if (item.PrescMedID == 0) continue;

                var med = _context.PrescriptionMedicines
                    .FirstOrDefault(m => m.PrescMedID == item.PrescMedID);
>>>>>>> main

                if (med != null)
                {
                    med.Status = item.Status;
<<<<<<< HEAD
                    med.Reason = item.Reason;
=======
                    med.Reason = item.Status == "Not Given" ? item.Reason : null;
>>>>>>> main
                    med.PatientConfirmed = true;
                }
            }

            _context.SaveChanges();
<<<<<<< HEAD
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
=======

            return Json(new { success = true });
        }

        // 📄 EXTERNAL PDF
        [HttpPost]
        public IActionResult GenerateExternal([FromBody] List<MedicineVM> medicines)
        {
            if (medicines == null || medicines.Count == 0)
                return BadRequest("No medicines selected");

            var model = new ExternalPrescriptionPdfModel
            {
                PatientName = "External Patient",
                NIC = "N/A",
                Medicines = medicines.Select(m => new MedicineVM
                {
                    MedicineName = m.MedicineName ?? "-",
                    Dosage = m.Dosage ?? "-",
                    Duration = m.Duration ?? "-",
                    TimesPerDay = m.TimesPerDay,
                    Reason = m.Reason ?? "-"
                }).ToList()
            };

            var doc = new ExternalPrescriptionDocument(model);
            var pdf = doc.GeneratePdf();

            return File(pdf, "application/pdf", "ExternalPrescription.pdf");
        }

        [HttpPost]
        public IActionResult SaveStatus(List<MedicineVM> medicines)
        {
            foreach (var m in medicines)
            {
                var dbMed = _context.PrescriptionMedicines
                    .FirstOrDefault(x => x.PrescMedID == m.PrescMedID);

                if (dbMed != null)
                {
                    dbMed.Status = m.Status;
                    dbMed.Reason = m.Status == "Given" ? null : m.Reason;
                }
            }

            _context.SaveChanges();

            return Json(new { success = true });
        }
    }
}
>>>>>>> main
