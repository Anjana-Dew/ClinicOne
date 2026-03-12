using ClinicOne.Data;
using ClinicOne.Models.ViewModels.Doctor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ClinicOne.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = "Doctor")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class PatientMedicalProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PatientMedicalProfileController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(string id)
        {
            // Vitasl and personal info
            var patient = _context.Patients.FirstOrDefault(p => p.PatientNIC == id);

            if (patient == null)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            decimal? bmi = null;

            if (patient.Height != null && patient.Weight != null) { 
                
                var heightM = (decimal)patient.Height / 100;
                bmi = patient.Weight / (heightM * heightM);
            }

            var model = new PatientMedicalProfileViewModel
            {
                PatientNIC = patient.PatientNIC,
                FullName = patient.FullName,
                BloodType = patient.BloodType,
                Address = patient.Address,
                PhoneNumber = patient.PhoneNumber,
                Height = patient.Height,
                Weight = patient.Weight,
                BloodPressure = patient.BloodPressure,
                BMI = bmi
            };


            // test reports
            var reports = _context.MedicalReports
                            .Where(r => r.PatientNIC == id)
                            .OrderByDescending(r => r.UploadedDate)
                            .ToList();

            var reportCards = new List<MedicalReportCardViewModel>();

            foreach (var report in reports)
            {
                var results = (from rtr in _context.ReportTestResults
                               join tp in _context.TestParameters on rtr.ParameterID equals tp.ParameterID
                               join panel in _context.TestPanels on tp.PanelID equals panel.PanelID
                               where rtr.ReportID == report.ReportID
                               select new
                               {
                                   panel.PanelID,
                                   panel.TestName,
                                   tp.ParameterName,
                                   tp.Unit,
                                   rtr.TestValue,
                                   rtr.ResultStatus
                               }).ToList();

                var panelId = results.FirstOrDefault()?.PanelID;
                var panelName = results.FirstOrDefault()?.TestName;

                if (panelId == null)
                    continue;

                // Skip if we already added this panel
                if (reportCards.Any(c => c.PanelName == panelName))
                    continue;

                var latestResults = results.Select(r => new ParameterResultViewModel
                {
                    ParameterName = r.ParameterName,
                    TestValue = r.TestValue,
                    Unit = r.Unit,
                    ResultStatus = r.ResultStatus
                }).ToList();

                // find previous report with SAME PANEL
                var previousReport = (from mr in _context.MedicalReports
                                      join rtr in _context.ReportTestResults on mr.ReportID equals rtr.ReportID
                                      join tp in _context.TestParameters on rtr.ParameterID equals tp.ParameterID
                                      where mr.PatientNIC == id
                                      && mr.UploadedDate < report.UploadedDate
                                      && tp.PanelID == panelId
                                      select mr)
                                      .Distinct()
                                      .OrderByDescending(r => r.UploadedDate)
                                      .FirstOrDefault();

                List<ParameterResultViewModel> previousResults = null;
                string previousPath = null;
                DateTime? previousDate = null;

                if (previousReport != null)
                {
                    previousResults = (from rtr in _context.ReportTestResults
                                       join tp in _context.TestParameters on rtr.ParameterID equals tp.ParameterID
                                       where rtr.ReportID == previousReport.ReportID
                                       select new ParameterResultViewModel
                                       {
                                           ParameterName = tp.ParameterName,
                                           TestValue = rtr.TestValue,
                                           Unit = tp.Unit,
                                           ResultStatus = rtr.ResultStatus
                                       }).ToList();

                    previousPath = previousReport.ReportPath;
                    previousDate = previousReport.UploadedDate;
                }

                reportCards.Add(new MedicalReportCardViewModel
                {
                    PanelName = panelName,
                    LatestUploadedDate = report.UploadedDate,
                    LatestReportPath = report.ReportPath,
                    LatestResults = latestResults,
                    PreviousUploadedDate = previousDate,
                    PreviousReportPath = previousPath,
                    PreviousResults = previousResults
                });

                if (reportCards.Count == 2)
                    break;
            }

            model.MedicalReports = reportCards;
            return View(model);
        }

        [HttpPost]
        public IActionResult UpdateHeight([FromBody] HeightUpdateRequest request)
        {
            var patient = _context.Patients.FirstOrDefault(p => p.PatientNIC == request.Nic);

            if(patient == null)
            {
                return NotFound();
            }

            patient.Height = request.Height;

            _context.SaveChanges();

            TempData["Success"] = "Height updated successfully";

            return Ok();
        }

        [HttpPost]
        public IActionResult UpdateWeight([FromBody] WeightUpdateRequest request) 
        {
            var patient = _context.Patients.FirstOrDefault(p => p.PatientNIC == request.Nic);

            if (patient == null) 
            { 
                return NotFound();
            }

            patient.Weight = request.Weight;

            _context.SaveChanges();
            TempData["Success"] = "Weight updated successfully";

            return Ok();
        }
        [HttpPost]
        public IActionResult UpdateBP([FromBody] BPUpdateRequest request)
        {
            var patient = _context.Patients.FirstOrDefault(p => p.PatientNIC == request.Nic);

            if (patient == null)
            {
                return NotFound();
            }

            patient.BloodPressure = request.Bp;

            _context.SaveChanges();

            TempData["Success"] = "Blood Pressure updated successfully";
            return Ok();
        }

        [HttpPost]
        public IActionResult UpdateBloodType([FromBody] BloodTypeRequest request)
        {
            var patient = _context.Patients.FirstOrDefault(p => p.PatientNIC == request.Nic);

            if (patient == null)
                return NotFound();

            if (patient.BloodType != null)
                return BadRequest();

            patient.BloodType = request.BloodType;

            _context.SaveChanges();

            return Ok();
        }

    }
}
