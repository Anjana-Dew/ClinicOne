using ClinicOne.Models.ViewModels.Patient;
using Microsoft.AspNetCore.Mvc;

namespace ClinicOne.Areas.Patient.Controllers
{
    [Area("Patient")]
    public class DashboardController : Controller
    {
        private static bool isFirstLogin = true;

        public IActionResult Index()
        {
            // Dashboard data (temporary until DB connection)

            var vm = new PatientDashboardViewModel
            {
                PatientName = "Kate Tanner",
                Gender = "Female",
                Age = 25,
                Height = 165,
                Weight = 60,
                BMI = 22.4M,
                BloodPressure = "120/80",
                ProgressStatus = "Stable",
                DoctorNotes = "Patient condition is stable. Continue medication.",

                NextSessionDate = DateTime.Now.AddDays(5),

                LatestReportName = "Blood Test",
                ReportStatus = "Normal",
                ReportPath = "/reports/sample-report.pdf",

                Medicines = new List<MedicineDto>
                {
                    new MedicineDto
                    {
                        Name = "Paracetamol",
                        Dosage = "Twice Daily"
                    },
                    new MedicineDto
                    {
                        Name = "Vitamin D",
                        Dosage = "Once Daily"
                    },
                    new MedicineDto
                    {
                        Name = "Iron Supplement",
                        Dosage = "After Meals"
                    }
                }
            };

            ViewBag.FirstLogin = isFirstLogin;

            return View(vm);
        }


        public IActionResult Reports()
        {
            return RedirectToAction("Index", "Reports", new { area = "Patient" });
        }


        public IActionResult Sessions()
        {
            return View("~/Areas/Patient/Views/Sessions/Index.cshtml");
        }


        public IActionResult Profile()
        {
            return View("~/Areas/Patient/Views/Profile/Index.cshtml");
        }


        public IActionResult Prescriptions()
        {
            return View("~/Areas/Patient/Views/Prescriptions/Index.cshtml");
        }
        public IActionResult Notes()
        {
            return View("~/Areas/Patient/Views/Notes/Index.cshtml");
        }

        [HttpGet]
        public IActionResult DisableFirstLogin()
        {
            isFirstLogin = false;
            return Ok();
        }

    }
}