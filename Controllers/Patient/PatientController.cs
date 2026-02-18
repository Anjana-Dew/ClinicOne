using Microsoft.AspNetCore.Mvc;

namespace ClinicOne.Controllers.Patient
{
    public class PatientController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
