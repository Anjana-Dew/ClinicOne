using Microsoft.AspNetCore.Mvc;

namespace ClinicOne.Controllers.Doctor
{
    public class DoctorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
