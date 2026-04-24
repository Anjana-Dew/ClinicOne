using ClinicOne.Data;
using ClinicOne.Models.ViewModels.Patient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicOne.Areas.Patient.Controllers
{
    [Area("Patient")]
    public class PrescriptionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PrescriptionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var nic = HttpContext.Session.GetString("PatientNIC");

            if (string.IsNullOrEmpty(nic))
                return RedirectToAction("Login", "Account");

            var prescriptions = await _context.Prescriptions
                .Where(p => p.PatientNIC == nic)
                .Include(p => p.PrescriptionMedicines)
                .OrderByDescending(p => p.PrescriptionDate)
                .ToListAsync();

            var model = prescriptions.Select(p => new PatientPrescriptionViewModel
            {
                PrescriptionID = p.PrescriptionID,
                PrescriptionDate = p.PrescriptionDate,

                PDFPath = _context.ExternalPrescriptions
                    .Where(e => e.PrescriptionID == p.PrescriptionID)
                    .Select(e => e.PDFPath)
                    .FirstOrDefault(),

                Medicines = p.PrescriptionMedicines.Select(m => new PatientPrescriptionMedicine
                {
                    MedicineName = m.MedicineName,
                    Status = m.Status,
                    Dosage = m.Dosage,
                    TimesPerDay = m.TimesPerDay,
                    PatientConfirmed = m.PatientConfirmed
                }).ToList()
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(int id)
        {
            var prescription = await _context.Prescriptions
                .Include(p => p.PrescriptionMedicines)
                .FirstOrDefaultAsync(p => p.PrescriptionID == id);

            if (prescription == null)
                return Json(new { success = false, message = "Prescription not found" });

            foreach (var med in prescription.PrescriptionMedicines)
            {
                med.PatientConfirmed = true;
            }

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "All medicines confirmed successfully"
            });
        }
    }
}