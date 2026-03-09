using Microsoft.AspNetCore.Mvc;

namespace ClinicOne.Areas.Pharmacist.Controllers
{
    public class SearchPatients : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

