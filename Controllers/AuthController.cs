using Microsoft.AspNetCore.Mvc;

namespace ClinicOne.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
