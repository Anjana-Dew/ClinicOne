using Microsoft.AspNetCore.Mvc;

namespace ClinicOne.Areas.Doctor.Controllers
{
    public class DoctorController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
