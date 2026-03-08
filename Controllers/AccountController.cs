using ClinicOne.Data;
using ClinicOne.Models.ViewModels.Auth;
using ClinicOne.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace ClinicOne.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context; 
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = _context.UserAccounts.FirstOrDefault(u => u.Username == model.Username);

            if (user == null)
            {
                ModelState.AddModelError("", "Invalid username or password. ");
                return View(model);
            }
            // check if locked
            if(user.IsLocked && user.LockUntil > DateTime.Now)
            {
                var remaining = (user.LockUntil.Value - DateTime.Now).Seconds;
                ModelState.AddModelError("", $"Account locked. Try again in {remaining} seconds.");
                return View(model);
            }
            // unlock after time passed
            if (user.IsLocked && user.LockUntil <= DateTime.Now) { 
                user.IsLocked = false;
                user.FailedAttempts = 0;
            }
            string hashedInput = PasswordService.HashPassword(model.Password); ;

            if (user.PasswordHash != hashedInput)
            {
                user.FailedAttempts++;

                if(user.FailedAttempts >= 3)
                {
                    user.IsLocked = true;
                    user.LockUntil = DateTime.Now.AddSeconds(30);
                }

                _context.SaveChanges();
                ModelState.AddModelError("", "Invalid username or password");
                return View(model);
            }

            // success

            user.FailedAttempts = 0;
            user.IsLocked = false;
            user.LastLogin = DateTime.Now;

            _context.SaveChanges();

            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role);
            HttpContext.Session.SetInt32("UserID", user.UserAccountID);

            if (user.FirstLogin)
            {
                return RedirectToAction("ChangePassword");
            }

            switch (user.Role) {
                case "Admin":
                    return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                case "Patient":
                    return RedirectToAction("Index", "Dashboard", new { area = "Patient" });
                case "Pharmacist":
                    return RedirectToAction("Index", "Dashboard", new { area = "Pharmacist" });
                case "Doctor":
                    return RedirectToAction("Index", "Dashboard", new { area = "Doctor" });
                default:
                    return Content("Role not implemented yet");
            }

        }
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = HttpContext.Session.GetInt32("UserID");

            if (userId == null)
                return RedirectToAction("Login");

            var user = _context.UserAccounts.Find(userId);

            if (user == null)
                return RedirectToAction("Login");

            user.PasswordHash = PasswordService.HashPassword(model.NewPassword);
            user.FirstLogin = false;

            _context.SaveChanges();

            TempData["Success"] = "Password changed successfully.";

            HttpContext.Session.Clear();

            TempData["Success"] = "Password changed successfully. Please login again.";

            return RedirectToAction("Login");
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
