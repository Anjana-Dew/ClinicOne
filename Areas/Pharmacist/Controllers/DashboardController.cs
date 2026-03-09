using Microsoft.AspNetCore.Mvc;

namespace ClinicOne.Areas.Pharmacist.Controllers
{
    [Area("Pharmacist")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Sessions()
        {
            return View("~/Areas/Pharmacist/Views/SearchPatients/Index.cshtml");
        }
    }
}