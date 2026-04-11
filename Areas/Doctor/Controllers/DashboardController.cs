using ClinicOne.Data;
using ClinicOne.Models.ViewModels.Doctor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicOne.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Authorize(Roles = "Doctor")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserID");
            var role = HttpContext.Session.GetString("Role");

            if (role != "Doctor")
            {
                return RedirectToAction("Login", "Account", new { area = "" });

            }

            if (userId == null)
            {
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            var doctor = _context.Doctors.FirstOrDefault(d => d.UserAccountID == userId);

            var model = new DoctorDashboardViewModel
            {
                DoctorName = doctor?.FullName,
                Specialization = doctor?.Specialization,
                CurrentDate = DateTime.Now,
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult SearchPatient(string nic)
        {
            var patient = _context.Patients.FirstOrDefault(p => p.PatientNIC == nic);

            if (patient == null)
            {
                TempData["Error"] = $"No patient found under NIC {nic}.";
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index", "PatientMedicalProfile", new { area = "Doctor", id = patient.PatientNIC });
        }
    }
}
