using ClinicOne.Data;
using ClinicOne.Models.ViewModels.Pharmacist;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ClinicOne.Areas.Pharmacist.Controllers
{
    [Area("Pharmacist")]
    public class PrescriptionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PrescriptionController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult GetPrescriptionByNIC(string nic)
        {
            if (string.IsNullOrWhiteSpace(nic))
                return Json(new { success = false, message = "NIC is required" });

            nic = nic.Trim();

            var patient = _context.Patients
                .Where(p => p.PatientNIC == nic)
                .Select(p => new
                {
                    name = p.FullName,
                    nic = p.PatientNIC
                })
                .FirstOrDefault();

            if (patient == null)
                return Json(new { success = false, message = "Patient not found" });

            var prescription = _context.Prescriptions
                .Where(p => p.PatientNIC == nic)
                .OrderByDescending(p => p.PrescriptionID)
                .FirstOrDefault();

            if (prescription == null)
                return Json(new { success = false, message = "No prescription found for this patient" });

            var medicines = _context.PrescriptionMedicines
                .Where(m => m.PrescriptionID == prescription.PrescriptionID)
                .Select(m => new
                {
                    medicineName = m.MedicineName ?? "-",
                    dosage = m.Dosage ?? "-",
                    status = m.Status ?? "-",
                    reason = m.Reason ?? "-",
                    duration = m.Duration ?? "-"
                })
                .ToList();

            return Json(new
            {
                success = true,
                patientName = patient.name,
                patientNIC = patient.nic,
                prescriptionID = prescription.PrescriptionID,
                medicines
            });
        }
    }
    }
    
