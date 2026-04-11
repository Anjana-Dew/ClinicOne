using ClinicOne.Data; // your DbContext namespace
using ClinicOne.Models.ViewModels.Patient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicOne.Areas.Patient.Controllers
{
    [Area("Patient")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Replace with logged-in patient NIC
            string patientNIC = "200680804080";

            var reports = await _context.MedicalReports
                .Where(r => r.PatientNIC == patientNIC)
                .Include(r => r.ReportTestResults)
                    .ThenInclude(res => res.Parameter)
                        .ThenInclude(p => p.Panel)
                .OrderByDescending(r => r.ReportDate)
                .ToListAsync();

            var reportVMs = reports.Select(r => new PatientReportViewModel
            {
                ReportID = r.ReportID,
                TestName = r.ReportTestResults.FirstOrDefault()?.Parameter.Panel.TestName ?? "Unknown",
                ReportDate = r.ReportDate,
                ReportPath = r.ReportPath,
                ReportStatus = r.ReportTestResults.Any() ? "Completed" : "Pending",
                Parameters = r.ReportTestResults.Select(res => new PatientReportParameter
                {
                    ParameterName = res.Parameter.ParameterName,
                    TestValue = res.TestValue.ToString(),
                    ResultStatus = res.ResultStatus
                }).ToList()
            }).ToList();

            ViewBag.NoResults = !reportVMs.Any();

            return View(reportVMs);
        }
    }
}

