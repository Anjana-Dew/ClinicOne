using ClinicOne.Data;
using Microsoft.AspNetCore.Mvc;

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
            ViewBag.Today = _context.Prescriptions.Count();
            ViewBag.Given = _context.PrescriptionMedicines.Count(m => m.Status == "Given");
            ViewBag.Pending = _context.PrescriptionMedicines.Count(m => m.Status != "Given");

            return View();
        }
    }
}