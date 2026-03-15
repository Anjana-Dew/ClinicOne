using ClinicOne.Data;
using ClinicOne.Models.ViewModels.Doctor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicOne.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = "Doctor")]
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string nic, string panel, int?year, int? month)
        {
            var query = (from mr in _context.MedicalReports
                           join rtr in _context.ReportTestResults on mr.ReportID equals rtr.ReportID
                           join tp in _context.TestParameters on rtr.ParameterID equals tp.ParameterID
                           join panelTbl in _context.TestPanels on tp.PanelID equals panelTbl.PanelID
                           where mr.PatientNIC == nic
                           select new ReportListViewModel
                           {
                               UploadedDate = mr.UploadedDate,
                               PanelName = panelTbl.TestName,
                               ReportPath = mr.ReportPath
                           })
                           .Distinct();
            // filter - panel
            if(!string.IsNullOrEmpty(panel) && panel != "All")
            {
                query = query.Where(r => r.PanelName == panel);
            }
            //filter- year
            if (year.HasValue)
            {
                query = query.Where(r => r.UploadedDate.Year == year.Value);
            }

            if (month.HasValue)
            {
                query = query.Where( r => r.UploadedDate.Month == month.Value);
            }

            var reports = query.OrderByDescending(r => r.UploadedDate).ToList();

            var panels = _context.TestPanels.Select(p => p.TestName).Distinct().ToList();

            ViewBag.Panels = panels;
            ViewBag.PatientNIC = nic;
            ViewBag.SelectedPanel = panel;
            ViewBag.SelectedYear = year;
            ViewBag.SelectedMonth = month;

            return View(reports);
        }
    }
}
