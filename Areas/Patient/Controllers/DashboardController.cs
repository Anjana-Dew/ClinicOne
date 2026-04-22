using ClinicOne.Data;
using ClinicOne.Models.ViewModels.Patient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicOne.Areas.Patient.Controllers
{
    [Area("Patient")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var nic = HttpContext.Session.GetString("PatientNIC");

            // 🔥 Recover session
            if (string.IsNullOrEmpty(nic))
            {
                var username = User.Identity?.Name;

                if (!string.IsNullOrEmpty(username))
                {
                    var userPatient = await _context.Patients
                        .Include(p => p.UserAccount)
                        .FirstOrDefaultAsync(p => p.UserAccount.Username == username);

                    if (userPatient != null)
                    {
                        nic = userPatient.PatientNIC;
                        HttpContext.Session.SetString("PatientNIC", nic);
                    }
                }
            }

            if (string.IsNullOrEmpty(nic))
                return RedirectToAction("Login", "Account");

            // ✅ GET PATIENT
            var patientEntity = await _context.Patients
                .FirstOrDefaultAsync(p => p.PatientNIC == nic);

            // ✅ PROGRESS
            var progress = await _context.PatientProgresses
                .Where(p => p.PatientNIC == nic)
                .OrderByDescending(p => p.ProgressDate)
                .FirstOrDefaultAsync();

            // ✅ REPORT (ONLY FIELDS THAT EXIST)
            var report = await _context.MedicalReports
                .Where(r => r.PatientNIC == nic)
                .OrderByDescending(r => r.UploadedDate)
                .FirstOrDefaultAsync();

            // ✅ SESSION
            var session = await _context.ClinicSchedules
                .Where(s => s.PatientNIC == nic && s.ClinicDate >= DateTime.Today)
                .Include(s => s.ClinicSession)
                .OrderBy(s => s.ClinicDate)
                .FirstOrDefaultAsync();

            // ✅ MEDICINES (LATEST ONLY)
            var medicines = await _context.PrescriptionMedicines
                .Where(m => m.Prescription.PatientNIC == nic && m.Status != "Not Given")
                .OrderByDescending(m => m.PrescMedID)
                .Take(3)
                .Select(m => new MedicineDto
                {
                    Name = m.MedicineName,
                    Dosage = m.Dosage + " (" + m.TimesPerDay + "x/day)"
                })
                .ToListAsync();

            var vm = new PatientDashboardViewModel
            {
                PatientName = patientEntity?.FullName ?? "Patient",
                NIC = patientEntity?.PatientNIC ?? "-",
                BloodType = string.IsNullOrEmpty(patientEntity?.BloodType) ? "Not Added" : patientEntity.BloodType,
                Address = patientEntity?.Address ?? "-",
                PhoneNumber = patientEntity?.PhoneNumber ?? "-",

                // ❗ YOUR DB DOES NOT HAVE THESE → KEEP SAFE
                Height = 0,
                Weight = 0,
                BMI = 0,
                BloodPressure = "N/A",

                ProgressStatus = progress?.ProgressStatus ?? "Stable",
                DoctorNotes = progress?.DoctorNotes ?? "-",

                NextSessionDate = session?.ClinicDate,
                NextSessionName = session?.ClinicSession?.SessionName ?? "-",
                SessionTime = session != null
                ? $"{DateTime.Today.Add(session.ClinicSession.StartTime):hh:mm tt} - {DateTime.Today.Add(session.ClinicSession.EndTime):hh:mm tt}"
                : "-",

                // ✅ ONLY USE EXISTING FIELDS
                ReportID = report?.ReportID,
                ReportDate = report?.UploadedDate,
                ReportStatus = report != null ? "Completed" : "Pending",
                ReportPath = report?.ReportPath ?? "#",

                Medicines = medicines
            };

            return View(vm);
        }
    }
}