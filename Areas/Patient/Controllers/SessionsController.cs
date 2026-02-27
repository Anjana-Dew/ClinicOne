using Microsoft.AspNetCore.Mvc;

namespace ClinicOne.Areas.Patient.Controllers
{
    public class SessionsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
