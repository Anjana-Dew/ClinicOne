using Microsoft.AspNetCore.Mvc;

namespace ClinicOne.Areas.Patient.Controllers
{
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
