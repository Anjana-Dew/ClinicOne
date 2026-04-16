using ClinicOne.Data;
using ClinicOne.Models.ViewModels;
using ClinicOne.Models.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicOne.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class TestReportsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TestReportsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SearchPatient(string nic) {

            var patient = _context.Patients.FirstOrDefault(x => x.PatientNIC == nic);

            if (patient == null) 
            {
                return Json(new { success = false });
            }

            return Json(new 
            { 
                success = true,
                name = patient.FullName,
                nic = patient.PatientNIC
            });
        }

        public IActionResult Upload(string nic) {
            var patient = _context.Patients.FirstOrDefault(x => x.PatientNIC == nic);

            if(patient == null)
            {
                return RedirectToAction("Index");
            }
            var model = new UploadTestReportViewModel
            {
                PatientNIC = nic,
                PatientName = patient.FullName,

                Panels = _context.TestPanels.Select(x => new TestPanelViewModel
                {
                    PanelID = x.PanelID,
                    TestName = x.TestName,
                }).ToList()
            };

            return View(model);
        }

        public IActionResult GetParameters(int panelId)
        {
            var parameters = _context.TestParameters
                .Where(x => x.PanelID == panelId)
                .Select(x => new TestParameterViewModel
                {
                    ParameterID = x.ParameterID,
                    ParameterName = x.ParameterName,
                    Unit = x.Unit
                }).ToList();

            return Json(parameters);
        }

        [HttpPost]
        public IActionResult SaveReport([FromForm] SaveTestReportRequest request, IFormFile pdfFile)
        {
            string filePath = "";

            if (pdfFile != null) 
            { 
                var fileName = Guid.NewGuid() + Path.GetExtension(pdfFile.FileName);

                filePath = "/uploads/reports/" + fileName;

                var path = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/uploads/reports",
                    fileName
                );

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    pdfFile.CopyTo(stream);
                }
            }
            var report = new Models.Entities.MedicalReport
            {
                PatientNIC = request.PatientNIC,
                ReportDate = DateTime.Today,
                ReportPath = filePath
            };

            _context.MedicalReports.Add(report);
            _context.SaveChanges();

            foreach(var item in request.TestValues)
            {
                var range = _context.TestRanges.FirstOrDefault(x => x.ParameterID == item.Key);

                string status = "";
                if (range != null)
                {
                    decimal value = item.Value;
                    if(range.CriticalLow != null && value < range.CriticalLow)
                    {
                        status = "Risk";
                    }
                    else if (range.CriticalHigh != null && value > range.CriticalHigh)
                    {
                        status = "Risk";
                    }
                    else if (value < range.ReferenceMin || value > range.ReferenceMax)
                    {
                        status = "High";
                    }
                    else
                    {
                        status = "Normal";
                    }

                }
                var result = new Models.Entities.ReportTestResult
                {
                    ReportID = report.ReportID,
                    ParameterID = item.Key,
                    TestValue = item.Value,
                    ResultStatus = status
                };
                _context.ReportTestResults.Add(result);
            }
            _context.SaveChanges();

            return Json(new { success = true });
        } 
    }
}
