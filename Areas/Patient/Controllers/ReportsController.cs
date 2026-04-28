using ClinicOne.Data;
using ClinicOne.Models.ViewModels.Patient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicOne.Areas.Patient.Controllers
{
    [Area("Patient")]
    [Route("Patient/Reports")]
    public class ReportsController : BaseController
    {
        public ReportsController(ApplicationDbContext context) : base(context)
        {

        }

        [Route("")]
        public async Task<IActionResult> Index(string? search, string? status, int? panelId, DateTime? date)
        {
            var nic = HttpContext.Session.GetString("PatientNIC");

            if (string.IsNullOrEmpty(nic))
            {
                ViewBag.NoSessionWarning = true;
                nic = "";
            }

            var reportsData = await _context.MedicalReports
                .Include(r => r.ReportTestResults)
                    .ThenInclude(rtr => rtr.TestParameter)
                        .ThenInclude(tp => tp.TestPanel)
                .OrderByDescending(r => r.ReportDate)
                .ToListAsync();

            //FILTER
            if (!string.IsNullOrEmpty(nic))
            {
                reportsData = reportsData
                    .Where(r => r.PatientNIC == nic)
                    .ToList();
            }

            var reports = reportsData.Select(r => new PatientReportViewModel
            {
                ReportID = r.ReportID,
                ReportDate = r.ReportDate,
                ReportPath = r.ReportPath,

                TestName = r.ReportTestResults
                    .Select(x => x.TestParameter.TestPanel.TestName)
                    .FirstOrDefault(),

                Parameters = r.ReportTestResults
                    .Select(x => new PatientReportParameter
                    {
                        ParameterName = x.TestParameter.ParameterName,
                        TestValue = x.TestValue.ToString(),
                        ResultStatus = x.ResultStatus
                    })

                    .ToList(),

                ReportStatus = r.ReportTestResults.Any()
                ? GetOverallStatus(r.ReportTestResults.Select(x => x.ResultStatus))
               : "Normal"
               }).ToList();


            //SEARCH FILTER
            if (!string.IsNullOrWhiteSpace(search))
            {
                reports = reports
                    .Where(x => x.TestName != null &&
                                x.TestName.ToLower().Contains(search.ToLower()))
                    .ToList();
            }

            //TEST TYPE FILTER
            if (panelId.HasValue)
            {
                var selectedPanel = await _context.TestPanels
                    .Where(p => p.PanelID == panelId)
                    .Select(p => p.TestName)
                    .FirstOrDefaultAsync();

                reports = reports
                    .Where(x => x.TestName == selectedPanel)
                    .ToList();
            }

            //STATUS FILTER
            if (!string.IsNullOrWhiteSpace(status))
            {
                reports = reports
                    .Where(x => x.ReportStatus == status)
                    .ToList();
            }

            //DATE FILTER
            if (date.HasValue)
            {
                reports = reports
                    .Where(x => x.ReportDate.Date == date.Value.Date)
                    .ToList();
            }

            ViewBag.NoResults = !reports.Any();
            ViewBag.TestPanels = await _context.TestPanels.ToListAsync();

            return View(reports);
        }
        private string GetOverallStatus(IEnumerable<string> statuses)
        {
            if (statuses.Contains("Risk"))
                return "Risk";

            if (statuses.Contains("High"))
                return "High";

            return "Normal";
        }

    }
}