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

            var data = await _context.Prescriptions
                .Where(p => p.PatientNIC == nic)
                .Include(p => p.PrescriptionMedicines)
                .OrderByDescending(p => p.PrescriptionDate)
                .ToListAsync();

            var list = data.Select(p => new PatientPrescriptionViewModel
            {
                PrescriptionID = p.PrescriptionID,
                PrescriptionDate = p.PrescriptionDate,

                PDFPath = _context.ExternalPrescriptions
                    .Where(e => e.PrescriptionID == p.PrescriptionID)
                    .Select(e => e.PDFPath)
                    .FirstOrDefault(),

                IsConfirmed = p.PrescriptionMedicines.All(m =>
                    m.Status == "Given" || m.Status == "Partially Given"),

                TotalMedicines = p.PrescriptionMedicines.Count,
                TakenMedicines = p.PrescriptionMedicines.Count(m => m.PatientConfirmed),

                Medicines = p.PrescriptionMedicines.Select(m => new PatientPrescriptionMedicine
                {
                    MedicineName = m.MedicineName,
                    Status = m.Status,
                    Dosage = m.Dosage ?? "-",
                    TimesPerDay = m.TimesPerDay,
                    PatientConfirmed = m.PatientConfirmed
                }).ToList()
            }).ToList();

            return View(list);
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(int id)
        {
            var meds = await _context.PrescriptionMedicines
                .Where(m => m.PrescriptionID == id)
                .ToListAsync();

            foreach (var m in meds)
                m.PatientConfirmed = true;

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}