using ClinicOne.Data;
using ClinicOne.Models.ViewModels.Patient;
using ClinicOne.Models.ViewModels.Pharmacist;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using static QuestPDF.Helpers.Colors;

namespace ClinicOne.Areas.Patient.Controllers
{
    [Area("Patient")]
    public class PrescriptionsController : BaseController
    {

        private static readonly Regex DurationRegex =
            new(@"(\d+)\s*(week|day)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public PrescriptionsController(ApplicationDbContext context) : base(context)
        {

        }

        private string? GetPatientNic() =>
            HttpContext.Session.GetString("PatientNIC");

        public async Task<IActionResult> Index()
        {
            var nic = GetPatientNic();
            if (string.IsNullOrEmpty(nic))
                return RedirectToAction("Login", "Account");

            var prescriptions = await _context.Prescriptions
                .Where(p => p.PatientNIC == nic)
                .Include(p => p.PrescriptionMedicines)
                .OrderByDescending(p => p.PrescriptionDate)
                .ToListAsync();

            var ids = prescriptions.Select(p => p.PrescriptionID).ToList();

            var pdfMap = await _context.ExternalPrescriptions
                .Where(x => ids.Contains(x.PrescriptionID))
                .GroupBy(x => x.PrescriptionID)
                .ToDictionaryAsync(
                    g => g.Key,
                    g => g.OrderByDescending(x => x.GeneratedDate)
                           .Select(x => x.PDFPath)
                           .FirstOrDefault());

            var model = new List<PatientPrescriptionViewModel>();

            foreach (var p in prescriptions)
            {
                pdfMap.TryGetValue(p.PrescriptionID, out var pdf);
                bool hasExternal = !string.IsNullOrEmpty(pdf);

                var allMeds = p.PrescriptionMedicines.ToList();

                var visibleMeds = allMeds
                    .Where(m => !(m.Status == "Given" && m.PatientConfirmed))
                    .ToList();

                bool allGiven = allMeds.Any() &&
                allMeds.All(m => m.Status == "Given" && m.PatientConfirmed);

                var maxDays = allMeds.Any()
                    ? allMeds.Max(m => ParseDurationToDays(m.Duration))
                    : 0;

                bool isPast = DateTime.Today > p.PrescriptionDate.AddDays(maxDays);

                bool pharmacyActed = allMeds.Any(m =>
                    m.Status == "Given" ||
                    m.Status == "Not Given" ||
                    m.Status == "Partially Given");

                bool needsExternal = allMeds.Any(m =>
                    m.Status == "Not Given" || m.Status == "Partially Given");

                bool allConfirmed = allMeds
                    .Where(m => m.Status == "Not Given" || m.Status == "Partially Given")
                    .All(m => m.PatientConfirmed);


                model.Add(new PatientPrescriptionViewModel
                {
                    PrescriptionID = p.PrescriptionID,
                    PrescriptionDate = p.PrescriptionDate,
                    Notes = p.Notes,

                    PDFPath = pdf,
                    HasExternalPrescription = hasExternal,

                    PharmacyPending = !pharmacyActed,
                    AllGiven = allGiven,

                    ShowConfirmButton = hasExternal && allMeds.Any(m => (m.Status == "Not Given" || m.Status == "Partially Given") && !m.PatientConfirmed),

                    ShowCompleted = hasExternal && (allConfirmed || !needsExternal),

                    IsPast = isPast,
                    IsActive = !isPast,

                    Medicines = visibleMeds.Select(m => new PatientPrescriptionMedicine
                    {
                        MedicineName = m.MedicineName,
                        Dosage = m.Dosage,
                        TimesPerDay = m.TimesPerDay,
                        Duration = m.Duration,
                        Status = m.Status,
                        Reason = m.Reason,
                        PatientConfirmed = m.PatientConfirmed
                    }).ToList(),
                });
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmExternal(int id)
        {
            var nic = GetPatientNic();
            if (string.IsNullOrEmpty(nic))
                return Json(new { success = false, message = "Not logged in." });

            var prescription = await _context.Prescriptions
                .Include(p => p.PrescriptionMedicines)
                .FirstOrDefaultAsync(p => p.PrescriptionID == id);

            if (prescription == null || prescription.PatientNIC != nic)
                return Json(new { success = false, message = "Prescription not found." });

            foreach (var med in prescription.PrescriptionMedicines
            .Where(m => m.Status == "Not Given" || m.Status == "Partially Given"))
            {
                med.PatientConfirmed = true;

                var reminder = _context.MedicineReminders
                    .FirstOrDefault(r => r.PrescMedID == med.PrescMedID);

                if (reminder != null)
                {
                    reminder.IsActive = true;
                }
            }

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = "Confirmed: you took the external prescription."
            });
        }

        private int ParseDurationToDays(string? duration)
        {
            if (string.IsNullOrWhiteSpace(duration)) return 0;

            var match = DurationRegex.Match(duration);
            if (!match.Success) return 0;

            int n = int.Parse(match.Groups[1].Value);

            return match.Groups[2].Value.StartsWith("week", StringComparison.OrdinalIgnoreCase)
                ? n * 7
                : n;
        }
    }
}