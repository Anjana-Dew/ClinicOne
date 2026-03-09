using Microsoft.AspNetCore.Mvc;

namespace ClinicOne.Areas.Patient.Controllers
{
    [Area("Patient")]
    public class DashboardController : Controller
    {
        private static bool isFirstLogin = true;
        public IActionResult Index()
        {
            ViewBag.FirstLogin = isFirstLogin;
            return View();
        }

        public IActionResult Reports()
        {
            return View("~/Areas/Patient/Views/Reports/Index.cshtml");
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

        [HttpGet]
        public IActionResult DisableFirstLogin()
        {
            isFirstLogin = false;
            return Ok();
        }

    }
}
