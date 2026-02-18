using Microsoft.AspNetCore.Mvc;

namespace ClinicOne.Controllers.Pharmacist
{
    public class PharmacistController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
