using ClinicOne.Data;
using ClinicOne.Models.Entities;
using ClinicOne.Models.ViewModels.Patient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Collections.Specialized.BitVector32;
using ClinicOne.Services;
using ClinicOne.Models.Entities;

namespace ClinicOne.Areas.Patient.Controllers
{
    [Area("Patient")]
    public class DashboardController : BaseController
    {
        public DashboardController(ApplicationDbContext context)
            : base(context)
        {
        }

        public async Task<IActionResult> Index()
        {
            var nic = HttpContext.Session.GetString("PatientNIC");

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


            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.PatientNIC == nic);

            var vitals = await _context.PatientVitals
                .Where(v => v.PatientNIC == nic)
                .OrderByDescending(v => v.RecordedDate)
                .FirstOrDefaultAsync();

            decimal bmi = 0;
            if (vitals != null && vitals.Height > 0)
            {
                var h = vitals.Height.Value / 100;
                bmi = vitals.Weight.Value / (h * h);
            }

            var session = await _context.ClinicSchedules
               .Where(s => s.PatientNIC == nic && s.ClinicDate >= DateTime.Today)
               .Include(s => s.ClinicSession)
               .OrderBy(s => s.ClinicDate)
               .FirstOrDefaultAsync();

            var progress = await _context.PatientProgresses
                .Where(p => p.PatientNIC == nic)
                .OrderByDescending(p => p.ProgressDate)
                .FirstOrDefaultAsync();

            var latestReport = await _context.MedicalReports
                .Where(r => r.PatientNIC == nic)
                .OrderByDescending(r => r.ReportDate)
                .FirstOrDefaultAsync();


            var latestPrescription = await _context.Prescriptions
                .Where(p => p.PatientNIC == nic)
                .OrderByDescending(p => p.PrescriptionDate)
                .FirstOrDefaultAsync();


            ReportGroupDto latestReportGroup = null;

            if (latestReport != null)
            {
                var results = await _context.ReportTestResults
                    .Include(r => r.TestParameter)
                        .ThenInclude(p => p.TestPanel)
                    .Where(r => r.ReportID == latestReport.ReportID)
                    .ToListAsync();

                latestReportGroup = new ReportGroupDto
                {
                    TestName = results.FirstOrDefault()?.TestParameter.TestPanel.TestName,
                    ReportDate = latestReport.ReportDate,
                    Results = results.Select(x => new ReportResultDto
                    {
                        Parameter = x.TestParameter.ParameterName,
                        Value = x.TestValue,
                        Status = x.ResultStatus
                    }).ToList()
                };
            }

            var medicines = new List<MedicineDto>();

            if (latestPrescription != null)
            {
                medicines = await _context.PrescriptionMedicines
                    .Where(m => m.PrescriptionID == latestPrescription.PrescriptionID)
                    .OrderByDescending(m => m.PrescMedID)
                    .Select(m => new MedicineDto
                    {
                        Name = m.MedicineName,
                        Dosage = m.Dosage + " (" + m.TimesPerDay + "x/day)"
                    })
                    .ToListAsync();
            }

            var model = new PatientDashboardViewModel
            {
                PatientName = patient?.FullName ?? "",
                NIC = patient?.PatientNIC ?? "",
                BloodType = patient?.BloodType ?? "Not Added",
                PhoneNumber = patient?.PhoneNumber ?? "-",
                Address = patient?.Address ?? "-",

                Height = vitals?.Height,
                Weight = vitals?.Weight,
                BMI = vitals != null && vitals.Height > 0
                ? Math.Round(bmi, 2)
                : null,
                BloodPressure = vitals != null
                ? $"{vitals.Systolic}/{vitals.Diastolic}"
                : null,

                ReportGroups = latestReportGroup != null
                    ? new List<ReportGroupDto> { latestReportGroup }
                    : new List<ReportGroupDto>(),

                NextSessionDate = session?.ClinicDate,
                NextSessionName = session?.ClinicSession?.SessionName ?? "-",
                SessionStartTime = session?.ClinicSession?.StartTime != null
    ? session.ClinicSession.StartTime.ToString()
    : "",

                SessionEndTime = session?.ClinicSession?.EndTime != null
    ? session.ClinicSession.EndTime.ToString()
    : "",

                ProgressStatus = progress?.ProgressStatus,
                DoctorNotes = progress?.DoctorNotes,


                ReportID = latestReport?.ReportID,
                ReportDate = latestReport?.ReportDate,
                ReportStatus = latestReport != null ? "Completed" : "Pending",
                ReportPath = latestReport?.ReportPath ?? "#",
                
                Medicines = medicines,

                PrescriptionStatus = latestPrescription != null ? "Available" : "None"
            };

            return View(model);
        }

    }
}