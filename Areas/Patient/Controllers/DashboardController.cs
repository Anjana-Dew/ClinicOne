using ClinicOne.Data;
using ClinicOne.Models.Entities;
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

<<<<<<< HEAD
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

            var patientEntity = await _context.Patients
                .FirstOrDefaultAsync(p => p.PatientNIC == nic);

=======
            if (string.IsNullOrEmpty(nic))
                return RedirectToAction("Login", "Account");

            // PATIENT
            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.PatientNIC == nic);

            // VITALS
            var vitals = await _context.PatientVitals
                .Where(v => v.PatientNIC == nic)
                .OrderByDescending(v => v.RecordedDate)
                .FirstOrDefaultAsync();

            decimal bmi = 0;
            if (vitals?.Height > 0 && vitals?.Weight > 0)
            {
                var h = vitals.Height.Value / 100;
                bmi = vitals.Weight.Value / (h * h);
            }

            // ✅ PROGRESS + NOTES
>>>>>>> main
            var progress = await _context.PatientProgresses
                .Where(p => p.PatientNIC == nic)
                .OrderByDescending(p => p.RecordedDate)
                .FirstOrDefaultAsync();

<<<<<<< HEAD
            var report = await _context.MedicalReports
                .Where(r => r.PatientNIC == nic)
                .OrderByDescending(r => r.UploadedDate)
                .FirstOrDefaultAsync();

=======
            // ✅ NEXT SESSION
>>>>>>> main
            var session = await _context.ClinicSchedules
                .Include(s => s.ClinicSession)
                .Where(s => s.PatientNIC == nic)
                .OrderBy(s => s.ClinicDate)
                .FirstOrDefaultAsync();

<<<<<<< HEAD
=======
            // ✅ MEDICINES (FIXED)
>>>>>>> main
            var medicines = await _context.PrescriptionMedicines
                .Include(m => m.Prescription)
                .Where(m => m.Prescription.PatientNIC == nic)
                .Select(m => new MedicineDto
                {
                    Name = m.MedicineName,
                    Dosage = m.Dosage + " (" + m.TimesPerDay + "x/day)"
                })
                .ToListAsync();

            // ✅ REPORTS (CORRECT RELATIONS)
            var reports = await _context.ReportTestResults
                .Include(r => r.TestParameter)
                    .ThenInclude(p => p.TestPanel)
                .Include(r => r.MedicalReport)
                .Where(r => r.MedicalReport.PatientNIC == nic)
                .Select(r => new ReportResultDto
                {
                    TestName = r.TestParameter.TestPanel.TestName,
                    Parameter = r.TestParameter.ParameterName,
                    Value = r.TestValue,
                    Status = r.ResultStatus
                })
                .ToListAsync();

            return View(new PatientDashboardViewModel
            {
                PatientName = patient?.FullName ?? "",
                NIC = patient?.PatientNIC ?? "",
                BloodType = patient?.BloodType,
                PhoneNumber = patient?.PhoneNumber,
                Address = patient?.Address,

<<<<<<< HEAD
                Height = 0,
                Weight = 0,
                BMI = 0,
                BloodPressure = "N/A",
=======
                Height = vitals?.Height ?? 0,
                Weight = vitals?.Weight ?? 0,
                BMI = Math.Round(bmi, 2),
                BloodPressure = vitals != null
                    ? $"{vitals.Systolic}/{vitals.Diastolic}"
                    : "N/A",
>>>>>>> main

                // ✅ FIXED DATA
                ProgressStatus = progress?.ProgressStatus ?? "No Data",
                DoctorNotes = progress?.DoctorNotes ?? "",

                NextSessionDate = session?.ClinicDate,
                NextSessionName = session?.ClinicSession?.SessionName ?? "",
                SessionTime = session != null
                    ? $"{session.ClinicSession.StartTime} - {session.ClinicSession.EndTime}"
                    : "",

<<<<<<< HEAD
                ReportID = report?.ReportID,
                ReportDate = report?.UploadedDate,
                ReportStatus = report != null ? "Completed" : "Pending",
                ReportPath = report?.ReportPath ?? "#",

                Medicines = medicines
            };

            return View(vm);
=======
                Medicines = medicines,
                ReportResults = reports
            });
>>>>>>> main
        }
    }
    }
