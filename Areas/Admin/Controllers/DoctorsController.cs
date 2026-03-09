using ClinicOne.Data;
using ClinicOne.Models.Entities;
using ClinicOne.Models.ViewModels.Admin;
using ClinicOne.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Security.Cryptography;
using System.Text;

namespace ClinicOne.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DoctorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DoctorsController(ApplicationDbContext context)
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
        public IActionResult Register(RegisterDoctorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            if (_context.Doctors.Any(d => d.RegistrationNumber == model.RegistrationNumber))
            {
                ModelState.AddModelError("", "Doctor already exists.");
                return View("Index", model);
            }

            string last4 = model.RegistrationNumber.Substring(model.RegistrationNumber.
                Length - 4);
            string generatedPassword = $"Clinic@{last4}";
            string hashedPassword = PasswordService.HashPassword(generatedPassword);

            var user = new UserAccount
            {
                Username = model.RegistrationNumber,
                PasswordHash = hashedPassword,
                Role = "Doctor",
                IsLocked = false,
                FailedAttempts = 0,
                FirstLogin = true
            };

            _context.UserAccounts.Add(user);
            _context.SaveChanges();

            var doctor = new ClinicOne.Models.Entities.Doctor
            {
                FullName = model.Name,
                RegistrationNumber = model.RegistrationNumber,
                Specialization = model.Specialization,
                UserAccountID = user.UserAccountID,
                IsActive = true
            };

            _context.Doctors.Add(doctor);
            _context.SaveChanges();

            TempData["SuccessMessage"] = $"Doctor registered successfully. Default Password: {generatedPassword}";

            return RedirectToAction("Index");
        }

        //search
        [HttpGet]
        public IActionResult SearchDoctor(string regNo)
        {
            var doctor = _context.Doctors.FirstOrDefault(d => d.RegistrationNumber == regNo);

            if (doctor == null)
            {
                return Json(new { success = false, message = "Doctor not found." });
            }

            return Json(new
            {
                success = true,
                name = doctor.FullName,
                regNo = doctor.RegistrationNumber,
                specialization = doctor.Specialization,
                isActive = doctor.IsActive
            });
        }
        //Deactivate
        [HttpPost]
        public IActionResult DeactivateDoctor(string regNo) {
            var doctor = _context.Doctors.FirstOrDefault(d => d.RegistrationNumber == regNo);

            if (doctor == null) {
                return Json(new { success = false });
            }
            doctor.IsActive = false;
            _context.SaveChanges();

            return Json(new { success = true });

        }

        //Activate
        [HttpPost]
        public IActionResult ActivateDoctor(string regNo) 
        { 
            var doctor = _context.Doctors.FirstOrDefault(d => d.RegistrationNumber == regNo);

            if (doctor == null) { 
                return Json(new { success = false });
            }

            doctor.IsActive = true;
            _context.SaveChanges();

            return Json(new { success = true });
        }

        // update
        [HttpPost]
        public IActionResult UpdateDoctor([FromBody] UpdateDoctorDto model)
        {
            var doctor = _context.Doctors.FirstOrDefault(d => d.RegistrationNumber == model.RegNo);

            if (doctor == null)
            {
                return Json(new { success = false });

            }
            doctor.FullName = model.Name;
            doctor.Specialization = model.Specialization;

            _context.SaveChanges();

            return Json(new { success = true });
        }
    }
}
