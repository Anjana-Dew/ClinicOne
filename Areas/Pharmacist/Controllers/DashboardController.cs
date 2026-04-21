using ClinicOne.Data;
using ClinicOne.Models.ViewModels.Pharmacist;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ClinicOne.Areas.Pharmacist.Controllers
{
    [Area("Pharmacist")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var today = DateTime.Today;

            ViewBag.TodayPrescriptions = _context.Prescriptions
                .Count(p => p.PrescriptionDate == today);

            ViewBag.GivenMedicines = _context.PrescriptionMedicines
                .Count(m => m.Status == "Given");

            ViewBag.PendingMedicines = _context.PrescriptionMedicines
                .Count(m => m.Status != "Given");

            return View();
        }

        
        
        
    }
}