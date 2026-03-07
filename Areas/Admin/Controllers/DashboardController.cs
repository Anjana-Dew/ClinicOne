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
            var pharmacistCount = _context.Pharmacists.Count();
            var adminCount = _context.Admins.Count();

            ViewBag.PatientCount = patientCount;
            ViewBag.DoctorCount = doctorCount;
            ViewBag.PharmacistCount = pharmacistCount;
            ViewBag.AdminCount = adminCount;

            var recentLogs = (from log in _context.AccessLogs
                              join d in _context.Doctors
                              on log.DoctorID equals d.DoctorID
                              orderby log.AccessDateTime descending
                              select new
                              {
                                  DoctorName = d.FullName,
                                  log.PatientNIC,
                                  log.Action,
                                  log.AccessDateTime
                              })
                              .Take(3)
                              .ToList();
            ViewBag.RecentLogs = recentLogs;

            return View();


        }
    }
}
