using ClinicOne.Data;
using Microsoft.AspNetCore.Mvc;

namespace ClinicOne.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context; 
        }
        public IActionResult Index()
        {
            var patientCount = _context.Patients.Count();
            var doctorCount = _context.Doctors.Count();

            ViewBag.PatientCount = patientCount;
            ViewBag.DoctorCount = doctorCount;
            return View();
        }
    }
}
