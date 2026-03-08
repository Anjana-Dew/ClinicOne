using ClinicOne.Data;
using ClinicOne.Models.Entities;
using ClinicOne.Models.ViewModels.Admin;
using ClinicOne.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System.Security.Cryptography;
using System.Text;

namespace ClinicOne.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private const string MAIN_ADMIN_EMAIL = "mainadmin@clinic.com";

        public AdminsController(ApplicationDbContext context)
        {
            _context = context;
        }
        private bool IsMainAdmin()
        {
            //return true;

            // when loggin is doen uncomment these
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
            ViewBag.IsMainAdmin = true;

            return View(new RegisterAdminViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterAdminViewModel model)
        {
            if (!IsMainAdmin())
            {
                return View("AccessDenied");
            }
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }
            if (_context.Admins.Any(a => a.Email == model.Email))
            {
                ModelState.AddModelError("", "Admin already exists.");
                return View("Index", model);
            }

            string defaultPassword = GenerateDefaultPassword(model.Email);
            string hash = PasswordService.HashPassword(defaultPassword);

            var user = new UserAccount
            {
                Username = model.Email,
                PasswordHash = hash,
                Role = "Admin",
                FirstLogin = true
            };

            _context.UserAccounts.Add(user);
            _context.SaveChanges();

            var admin = new ClinicOne.Models.Entities.Admin
            {
                UserAccountID = user.UserAccountID,
                Name = model.Name,
                Email = model.Email
            };

            _context.Admins.Add(admin);
            _context.SaveChanges();

            TempData["SuccessMessage"] = $"Admin registered. Default password: {defaultPassword}";

            return RedirectToAction("Index");
        }

        public IActionResult Search(string email)
        {
            var admin = _context.Admins.FirstOrDefault(a => a.Email == email);

            if(admin == null)
            {
                return Json(null);
            }
            return Json(new
            {
                admin.Name,
                admin.Email,
                admin.IsActive
            });
        }

        [HttpPost]
        public IActionResult DeactivateAdmin(string email)
        {
            if (!IsMainAdmin())
                return Json(new { success = false });

            if (email.Equals(MAIN_ADMIN_EMAIL, StringComparison.OrdinalIgnoreCase))
                return Json(new { success = false, message = "Main admin cannot be deactivated." });

            var admin = _context.Admins.FirstOrDefault(a => a.Email == email);

            if (admin == null)
                return Json(new { success = false });

            admin.IsActive = false;

            _context.SaveChanges();

            return Json(new { success = true });
        }

        [HttpPost]
        public IActionResult ActivateAdmin(string email)
        {
            if (!IsMainAdmin())
                return Json(new { success = false });

            var admin = _context.Admins.FirstOrDefault(a => a.Email == email);

            if (admin == null)
                return Json(new { success = false });

            admin.IsActive = true;

            _context.SaveChanges();

            return Json(new { success = true });
        }

        private string GenerateDefaultPassword(string email)
        {
            var prefix = email.Substring(0, 4).ToUpper();
            return $"Clinic@{prefix}";
        }
    }
}
