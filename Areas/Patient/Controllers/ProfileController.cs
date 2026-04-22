using ClinicOne.Data;
using ClinicOne.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicOne.Areas.Patient.Controllers
{
    [Area("Patient")]
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Update(ClinicOne.Models.Entities.Patient model)
        {
            var nic = HttpContext.Session.GetString("PatientNIC");

            if (string.IsNullOrEmpty(nic))
                return RedirectToAction("Login", "Account");

            var patient = await _context.Patients
                .FirstOrDefaultAsync(p => p.PatientNIC == nic);

            if (patient == null)
                return NotFound();

            // UPDATE ONLY FIELDS YOU NEED
            patient.FullName = model.FullName?.Trim();
            patient.PhoneNumber = model.PhoneNumber?.Trim();
            patient.BloodType = model.BloodType;
            patient.Address = model.Address;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Profile updated successfully";

            return RedirectToAction("Index", "Dashboard");
        }
    }
}