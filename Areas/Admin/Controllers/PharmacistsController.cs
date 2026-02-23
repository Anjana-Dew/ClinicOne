using Microsoft.AspNetCore.Mvc;

namespace ClinicOne.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PharmacistsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
