using Microsoft.AspNetCore.Mvc;

namespace ClinicOne.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account", new {area = ""});
        }
        public IActionResult Test()
        {
            return Content("Account Controller Works");
        }
    }
}
