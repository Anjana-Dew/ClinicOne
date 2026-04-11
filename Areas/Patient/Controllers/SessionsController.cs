using ClinicOne.Data;
using ClinicOne.Models.ViewModels.Patient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ClinicOne.Areas.Patient.Controllers
{
    [Area("Patient")]
    public class SessionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SessionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        public async Task<IActionResult> Index(string patientNIC)
        {
            if (string.IsNullOrEmpty(patientNIC))
            {
                return BadRequest("PatientNIC is required.");
            }

            // Fetch all schedules for the patient including session info
            var schedules = await _context.ClinicSchedules
                .Where(cs => cs.PatientNIC == patientNIC)
                .Include(cs => cs.Session)
                .OrderBy(cs => cs.ClinicDate)
                .ToListAsync();

            if (!schedules.Any())
            {
                return View(Enumerable.Empty<PatientSessionViewModel>());
            }

            // Determine the first upcoming session
            var today = DateTime.Today;
            var nextSchedule = schedules.FirstOrDefault(s => s.ClinicDate >= today);

            // Map to ViewModel
            var sessionVMs = schedules.Select(s => new PatientSessionViewModel
            {
                ScheduleID = s.ScheduleID,
                SessionName = s.Session?.SessionName ?? "Unknown",
                ClinicDate = s.ClinicDate,
                StartTime = s.Session?.StartTime ?? TimeSpan.Zero,
                EndTime = s.Session?.EndTime ?? TimeSpan.Zero,
                IsNextSession = nextSchedule != null && s.ScheduleID == nextSchedule.ScheduleID
            }).ToList();

            return View(sessionVMs);
        }
    }
}