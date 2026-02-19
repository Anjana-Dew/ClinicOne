using Microsoft.AspNetCore.Mvc;

namespace ClinicOne.Areas.Pharmacist.Controllers
{
    public class PharmacistController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
