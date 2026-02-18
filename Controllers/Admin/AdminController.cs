using Microsoft.AspNetCore.Mvc;

namespace ClinicOne.Controllers.Admin
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
