using Microsoft.AspNetCore.Mvc;

namespace ClinicOne.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
