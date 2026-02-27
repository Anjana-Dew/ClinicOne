using Microsoft.AspNetCore.Mvc;

namespace ClinicOne.Areas.Patient.Controllers
{
    public class PrescriptionsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
