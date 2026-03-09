using ClinicOne.Data;
using ClinicOne.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClinicOne.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PasswordManagementController : Controller
    {
        private readonly ApplicationDbContext _context;

        private const string MAIN_ADMIN_EMAIL = "mainadmin@clinic.com";

        public PasswordManagementController(ApplicationDbContext context)
        {
            _context = context;
        }
        private bool IsMainAdmin()
        {
   
            var username = HttpContext.Session.GetString("Username");

            return username != null &&
                username.Equals(MAIN_ADMIN_EMAIL, StringComparison.OrdinalIgnoreCase);
        }
        public IActionResult Index()
        {
            if (!IsMainAdmin())
            {
                return View("AccessDenied");
            }
            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword(string username, string newPassword)
        {
            if (!IsMainAdmin())
            {
                return View("AccessDenied");
            }

            if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(newPassword))
            {
                TempData["Error"] = "Username and password are required.";
                return RedirectToAction("Index");
            }
            if (newPassword.Length < 8)
            {
                TempData["Error"] = "Password must be at least 8 characters.";
                return RedirectToAction("Index");
            }
            var user = _context.UserAccounts.FirstOrDefault(u => u.Username == username);

            if (user == null) 
            {
                TempData["Error"] = "No user found under the given username";
                return RedirectToAction("Index");
            }

            user.PasswordHash = PasswordService.HashPassword(newPassword);
            user.FirstLogin = true;

            _context.SaveChanges();

            TempData["Success"] = "Password reset successfully. User must change password on next login.";

            return RedirectToAction("Index");
        }
    }
}
