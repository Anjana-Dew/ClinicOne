using ClinicOne.Data;
using ClinicOne.Models.ViewModels.Doctor;
using ClinicOne.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicOne.Areas.Doctor.Controllers
{

    [Area("Doctor")]
    [Authorize(Roles = "Doctor")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public class VitalsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public VitalsController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(string patientNIC)
        {
            var vitals = _context.PatientVitals
                .Where(v => v.PatientNIC == patientNIC)
                .OrderBy(v => v.RecordedDate)
                .ToList();

            var result = new List<VitalHistoryRowViewModel>();

            decimal? lastHeight = null;
            decimal? lastWeight = null;
            int? lastSys = null;
            int? lastDia = null;

            foreach (var v in vitals)
            {
                if (v.Height != null) lastHeight = v.Height;
                if (v.Weight != null) lastWeight = v.Weight;
                if (v.Systolic != null) lastSys = v.Systolic;
                if (v.Diastolic != null) lastDia = v.Diastolic;

                result.Add(new VitalHistoryRowViewModel
                {
                    RecordedDate = v.RecordedDate,
                    Height = lastHeight,
                    Weight = lastWeight,
                    Systolic = lastSys,
                    Diastolic = lastDia
                });
            }
            result = result.OrderByDescending(r => r.RecordedDate).ToList();

            return View(result);
        }
    }
}
