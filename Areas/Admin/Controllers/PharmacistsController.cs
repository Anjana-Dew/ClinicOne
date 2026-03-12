using ClinicOne.Data;
using ClinicOne.Models.Entities;
using ClinicOne.Models.ViewModels.Admin;
using ClinicOne.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace ClinicOne.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class PharmacistsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PharmacistsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        //Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterPharmacistViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            //Check duplicates
            if(_context.Pharmacists.Any(p => p.RegistrationNumber == model.RegistrationNumber))
            {
                ModelState.AddModelError("", "Pharmacist already exists.");
                return View("Index", model);
            }

            // generate password
            string last4 = model.RegistrationNumber.Substring(model.RegistrationNumber.Length - 4);
            string generatedPassword = $"Clinic@{last4}";
            string hashedPassword = PasswordService.HashPassword(generatedPassword);

            var user = new UserAccount
            {
                Username = model.RegistrationNumber,
                PasswordHash = hashedPassword,
                Role = "Pharmacist",
                IsLocked = false,
                FailedAttempts = 0,
                FirstLogin = true
            };
            _context.UserAccounts.Add(user);
            _context.SaveChanges();

            var pharmacist = new ClinicOne.Models.Entities.Pharmacist
            {
                Name = model.Name,
                RegistrationNumber = model.RegistrationNumber,
                UserAccountID = user.UserAccountID,
                IsActive = true
            };
            _context.Pharmacists.Add(pharmacist);
            _context.SaveChanges();

            TempData["SuccessMessage"] = $"Pharmacist registered successfully. Default Password: {generatedPassword}";

            return RedirectToAction("Index");
        }

        //Search
        [HttpGet]
        public IActionResult SearchPharmacist(string regNo)
        {
            var pharmacist = _context.Pharmacists.FirstOrDefault(p => p.RegistrationNumber == regNo);

            if(pharmacist == null)
            {
                return Json(new { success = false, message = "Pharmacist not found." });
            }
            return Json(new
            {
                success = true,
                name = pharmacist.Name,
                regNo = pharmacist.RegistrationNumber,
                isActive = pharmacist.IsActive
            });
        }

        //Deactivate
        [HttpPost]
        public IActionResult DeactivatePharmacist(string regNo)
        {
            var pharmacist = _context.Pharmacists.FirstOrDefault(p => p.RegistrationNumber == regNo);

            if (pharmacist == null)
            {
                return Json(new { success = false });
            }

            pharmacist.IsActive = false;
            _context.SaveChanges() ;

            return Json(new { success = true });
        }

        //Activate
        [HttpPost]
        public IActionResult ActivatePharmacist(string regNo)
        {
            var pharmacist = _context.Pharmacists.FirstOrDefault(p => p.RegistrationNumber == regNo);

            if (pharmacist == null)
            {
                return Json(new { success = false });
            }

            pharmacist.IsActive = true;
            _context.SaveChanges();

            return Json(new { success = true });
        }
    }
}
