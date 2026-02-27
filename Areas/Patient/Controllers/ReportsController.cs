using Microsoft.AspNetCore.Mvc;

namespace ClinicOne.Areas.Patient.Controllers
{
    public class ReportsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
