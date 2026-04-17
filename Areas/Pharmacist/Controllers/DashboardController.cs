using ClinicOne.Data;
using ClinicOne.Models.ViewModels.Pharmacist;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ClinicOne.Areas.Pharmacist.Controllers
{
    [Area("Pharmacist")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Load dashboard page
        public IActionResult Index()
        {
            var today = DateTime.Today;

            // Total prescriptions created today
            ViewBag.TodayPrescriptions = _context.Prescriptions
                .Count(p => p.PrescriptionDate == today);

            // Total medicines already given
            ViewBag.GivenMedicines = _context.PrescriptionMedicines
                .Count(m => m.Status == "Given");

            // Total pending medicines (Not Given or Partially Given)
            ViewBag.PendingMedicines = _context.PrescriptionMedicines
                .Count(m => m.Status == "Not Given" || m.Status == "Partially Given");

            return View();
        

        }

        // Search patient by NIC
        public IActionResult Search(string nic)
        {
            // Find patient
            var patient = _context.Patients
                .Where(p => p.PatientNIC == nic)
                .Select(p => new
                {
                    patientName = p.FullName,   // MUST match JS
                    patientNIC = p.PatientNIC   // MUST match JS
                })
                .FirstOrDefault();

            if (patient == null)
                return Json(new { success = false, message = "Patient not found" });

            // Get latest prescription
            var prescription = _context.Prescriptions
                .Where(p => p.PatientNIC == nic)
                .OrderByDescending(p => p.PrescriptionDate)
                .FirstOrDefault();

            if (prescription == null)
            {
                return Json(new
                {
                    success = false,
                    message = "No prescription found",
                    patientName = patient.patientName,  // MUST match JS
                    patientNIC = patient.patientNIC,
                    prescriptionID = "N/A",
                    medicines = new List<object>()
                });
            }

            // Get medicines
            var medicines = _context.PrescriptionMedicines
                .Where(m => m.PrescriptionID == prescription.PrescriptionID)
                .Select(m => new
                {
                    PrescMedID = m.PrescMedID,
                    MedicineName = m.MedicineName ?? "",
                    Dosage = m.Dosage ?? "",
                    Status = m.Status ?? "",
                    Reason = m.Reason ?? ""
                })
                .ToList();

            return Json(new
            {
                success = true,
                patientName = patient.patientName,  // MUST match JS
                patientNIC = patient.patientNIC,
                prescriptionID = prescription.PrescriptionID,
                medicines = medicines
            });
        }
        [HttpPost]
        public IActionResult ConfirmPrescription([FromBody] List<MedicineItemViewModel> medicines)
        {
            if (medicines == null || medicines.Count == 0)
            {
                return Json(new { success = false });
            }

            foreach (var item in medicines)
            {
                var med = _context.PrescriptionMedicines
                    .FirstOrDefault(m => m.PrescMedID == item.PrescMedID);

                if (med != null)
                {
                    if (item.Status == "Not Given" && string.IsNullOrEmpty(item.Reason))
                    {
                        return Json(new { success = false, message = "Reason required" });
                    }

                    med.Status = item.Status;
                    med.Reason = item.Reason;
                    med.PatientConfirmed = true;
                }
            }

            _context.SaveChanges();

            return Json(new { success = true });
        }
    }
}